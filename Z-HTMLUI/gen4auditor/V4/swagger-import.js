// CockyAuditor: "Add API from Swagger"
// Prompts for the API URL and Swagger username/password, downloads swagger.json
// (Basic auth), lists every operation, and imports them as endpoints (/api/Apiaudit)
// under a host (/api/Apihosts). Also used to re-sync an existing host.
(function () {
  const { api, esc, toast } = Cocky;
  const METHODS = ["get", "post", "put", "patch", "delete", "head", "options"];
  const SPEC_PATHS = ["/swagger/v1/swagger.json", "/swagger.json", "/openapi.json", "/swagger/v2/swagger.json", "/v3/api-docs", "/swagger/docs/v1"];
  const blank = v => v == null || String(v).trim() === "" || String(v).trim().toLowerCase() === "string";
  const trimSlash = s => String(s || "").replace(/\/+$/, "");

  // ---------- fetching ----------
  async function fetchSpec(specUrl, user, pass) {
    const headers = { Accept: "application/json" };
    if (user || pass) headers.Authorization = "Basic " + btoa(unescape(encodeURIComponent(`${user}:${pass}`)));
    const ctrl = new AbortController();
    const t = setTimeout(() => ctrl.abort(), Cocky.CFG.requestTimeoutMs);
    let res;
    try { res = await fetch(specUrl, { headers, signal: ctrl.signal, cache: "no-store" }); }
    catch (e) {
      const err = new Error(e.name === "AbortError" ? "Timed out." :
        "The browser couldn't read it. The API has to allow this page in CORS (including the Authorization header), and let OPTIONS requests through before its Swagger login check.");
      err.kind = "network"; throw err;
    } finally { clearTimeout(t); }
    if (res.status === 401 || res.status === 403) { const e = new Error(`Swagger rejected the username or password (HTTP ${res.status}).`); e.kind = "auth"; throw e; }
    if (!res.ok) { const e = new Error(`HTTP ${res.status}`); e.kind = "http"; e.status = res.status; throw e; }
    const text = await res.text();
    try { return JSON.parse(text); }
    catch { const e = new Error("That URL didn't return JSON (it may be the Swagger UI page, not swagger.json)."); e.kind = "format"; throw e; }
  }

  // Try the given path first, then the usual locations.
  async function findSpec(root, path, user, pass, log) {
    const tries = [...new Set([path, ...SPEC_PATHS].filter(Boolean))].map(p => /^https?:/i.test(p) ? p : trimSlash(root) + "/" + p.replace(/^\/+/, ""));
    let last;
    for (const url of tries) {
      log(`Trying ${url}…`);
      try { return { spec: await fetchSpec(url, user, pass), url }; }
      catch (e) { last = e; if (e.kind === "auth" || e.kind === "network") throw e; }
    }
    throw last || new Error("No swagger.json found.");
  }

  // ---------- parsing (OpenAPI 3 and Swagger 2) ----------
  function parseSpec(spec, specUrl) {
    if (!spec || typeof spec !== "object" || !spec.paths) throw new Error("This doesn't look like a Swagger/OpenAPI document (no \"paths\").");
    const from = specUrl ? new URL(specUrl) : null;
    let origin, basePath = "";
    if (spec.openapi) {
      const s = (spec.servers || []).map(x => x.url).find(Boolean);
      if (s && /^https?:\/\//i.test(s)) { const u = new URL(s); origin = u.origin; basePath = trimSlash(u.pathname); }
      else { origin = from?.origin; basePath = trimSlash(s || ""); }
    } else {
      const scheme = (spec.schemes || [])[0] || from?.protocol.replace(":", "") || "https";
      origin = spec.host ? `${scheme}://${spec.host}` : from?.origin;
      basePath = trimSlash(spec.basePath || "");
    }
    const schemes = spec.components?.securitySchemes || spec.securityDefinitions || {};
    const schemeLabel = name => {
      const s = schemes[name] || {};
      return s.scheme ? `${s.type}:${s.scheme}` : s.type ? `${s.type}${s.in ? ":" + s.in : ""}` : name;
    };
    const endpoints = [];
    for (const [path, item] of Object.entries(spec.paths)) {
      for (const m of METHODS) {
        const op = item?.[m]; if (!op) continue;
        const sec = op.security ?? spec.security ?? [];
        const optional = sec.some(s => Object.keys(s).length === 0);
        const names = [...new Set(sec.flatMap(s => Object.keys(s)))];
        endpoints.push({
          method: m.toUpperCase(), path,
          operationId: op.operationId || null,
          summary: op.summary || op.description || "",
          tag: (op.tags || [])[0] || "",
          auth: names.length > 0 && !optional,
          authType: names.map(schemeLabel).join(", "),
          deprecated: !!op.deprecated
        });
      }
    }
    return {
      title: spec.info?.title || "", version: spec.info?.version || "",
      specVersion: spec.openapi || spec.swagger || "",
      origin, basePath, rootUrl: trimSlash((origin || "") + basePath), endpoints
    };
  }

  // ---------- endpoint record ----------
  function endpointRecord(op, parsed, host, details = {}) {
    const f = Cocky.SCHEMA.apiaudit.fields;
    const rec = {};
    for (const [k, t] of Object.entries(f)) if (k !== "id") rec[k] = t === "bool" ? false : null;
    const u = new URL(parsed.origin);
    const now = new Date().toISOString();
    return Object.assign(rec, {
      description: `${op.method} ${op.path}`,
      family: op.tag || (blank(host.family) ? null : host.family),
      url: op.path,
      apiRoot: parsed.rootUrl + "/",
      type: op.method,
      endpointType: op.method,
      apiHostId: host.id,
      hostName: host.apiHostName || u.hostname,
      fqdn: u.hostname,
      primaryPort: u.port ? +u.port : (u.protocol === "https:" ? 443 : 80),
      sslSupported: u.protocol === "https:",
      httpSupported: u.protocol === "http:",
      hasAuthEnabled: op.auth,
      authType: op.authType || null,
      applicationName: parsed.title || null,
      swaggerOperationId: op.operationId,
      swaggerVersion: [parsed.specVersion, parsed.version].filter(Boolean).join(" / ") || null,
      notes: [op.summary, op.deprecated ? "(deprecated in Swagger)" : ""].filter(Boolean).join(" ") || null,
      isActive: true,
      endpointSecure: false,
      createdDate: now, modifiedDate: now
    }, details);
  }
  // Fields that describe the API/ownership rather than one operation. These are copied from a
  // template endpoint on import. Everything else comes from Swagger.
  const PER_OPERATION = new Set(["id", "description", "url", "type", "endpointType", "family", "swaggerOperationId",
    "hasAuthEnabled", "authType", "notes", "createdDate", "modifiedDate", "lastAuditDate", "auditResultId",
    "endpointSecure", "isActive", "apiHostId", "apiRoot", "sslSupported", "httpSupported", "swaggerVersion", "applicationName"]);
  // The most useful ones are shown for editing in the wizard
  const DETAIL_FIELDS = ["auditorId", "auditorName", "auditorEmail", "auditorDepartment", "businessUnitName", "techContactName",
    "techContactEmail", "techContactPhone", "securityEmail", "securityPhone", "dbaName", "databaseType", "databaseFramework",
    "environment", "osType", "frameworkVersion", "buildingName", "groupDescription", "azureResourceGroup", "sourceRepository",
    "hashType", "securityClassification"];
  const copyable = tpl => Object.fromEntries(Object.entries(tpl || {}).filter(([k, v]) => !PER_OPERATION.has(k) && !blank(v)));
  // ---- Host defaults ----
  // Host fields that mean the same thing on an endpoint are copied onto every endpoint registered from that host.
  // Host-only fields (URL, Swagger login, audit status) and per-operation fields are never copied.
  const HOST_ONLY = new Set(["id", "apiHostName", "apiHostUrl", "swaggerUsername", "swaggerPassword", "isSecure",
    "lastAuditDate", "lastAuditId", "active", "isActive", "createdDate", "modifiedDate"]);
  const STRICT_PER_OP = new Set([...PER_OPERATION].filter(k => !["applicationName", "sslSupported", "httpSupported"].includes(k)));
  const ZERO_IS_UNSET = /(^|[a-z])Id$/;   // auditorId, groupId, buildingId, dbaId, businessUnitOwnerId: 0 = not set
  function hostDefaults(host) {
    const epFields = Cocky.SCHEMA.apiaudit.fields, out = {};
    for (const [k, v] of Object.entries(host || {})) {
      if (HOST_ONLY.has(k) || STRICT_PER_OP.has(k) || !(k in epFields) || blank(v)) continue;
      if (typeof v === "number" && v === 0 && ZERO_IS_UNSET.test(k)) continue;
      if (typeof v === "boolean" && !v) continue;          // only "true" says something; false is the default
      out[k] = v;
    }
    return out;
  }
  // Template endpoint first, host defaults on top (the host is the source of truth)
  const detailsFor = (host, tpl) => ({ ...copyable(tpl), ...hostDefaults(host) });

  const opKey = (method, path) => `${String(method || "GET").toUpperCase()} ${String(path || "").toLowerCase()}`;

  // POST that fills fields the API insists on (see Cocky.api.createFilling in app.js)
  async function createWithRetry(resource, body, filled) {
    const { data, filled: f } = await api.createFilling(resource, body);
    f.forEach(k => filled.add(k));
    return data;
  }

  // ---------- wizard UI ----------
  let modal;
  function ensureModal() {
    if (modal) return modal;
    document.body.insertAdjacentHTML("beforeend", `
    <div class="modal fade" id="swaggerModal" tabindex="-1" data-bs-backdrop="static"><div class="modal-dialog modal-xl modal-dialog-scrollable"><div class="modal-content">
      <div class="modal-header"><h5 class="modal-title">Add API from Swagger</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
      <div class="modal-body">
        <!-- step 1 -->
        <div data-step="1">
          <p class="text-muted small mb-3">Enter the API's address and its Swagger login. CockyAuditor downloads <code>swagger.json</code> and lists every endpoint so you can import them for auditing.</p>
          <div class="row g-3">
            <div class="col-md-7"><label class="form-label small mb-1" for="sw_root">API URL</label>
              <input class="form-control" id="sw_root" placeholder="https://myapi.azurewebsites.net" autocomplete="url"></div>
            <div class="col-md-5"><label class="form-label small mb-1" for="sw_name">Name <span class="text-muted">(optional)</span></label>
              <input class="form-control" id="sw_name" placeholder="Taken from Swagger if blank"></div>
            <div class="col-md-4"><label class="form-label small mb-1" for="sw_user">Swagger username</label>
              <input class="form-control" id="sw_user" autocomplete="off"></div>
            <div class="col-md-4"><label class="form-label small mb-1" for="sw_pass">Swagger password</label>
              <div class="input-group"><input class="form-control" id="sw_pass" type="password" autocomplete="new-password">
              <button class="btn btn-outline-secondary" type="button" id="sw_show">Show</button></div></div>
            <div class="col-md-4"><label class="form-label small mb-1" for="sw_path">swagger.json path</label>
              <input class="form-control" id="sw_path" value="/swagger/v1/swagger.json">
              <div class="form-text">Other common locations are tried automatically.</div></div>
            <div class="col-12"><div class="form-check">
              <input class="form-check-input" type="checkbox" id="sw_save" checked>
              <label class="form-check-label small" for="sw_save">Save the Swagger login on the host, so re-syncing doesn't ask again</label></div></div>
          </div>
          <details class="mt-3"><summary class="small">Can't reach it? Paste or upload swagger.json instead</summary>
            <div class="mt-2"><input type="file" class="form-control form-control-sm mb-2" id="sw_file" accept=".json,application/json">
            <textarea class="form-control font-monospace small" id="sw_paste" rows="5" placeholder='{"openapi":"3.0.1", ...}'></textarea>
            <div class="form-text">Still fill in the API URL above; it's used as the address for the endpoints.</div></div>
          </details>
          <div class="mt-3 small text-muted" id="sw_log"></div>
          <div class="alert alert-danger small mt-2 d-none" id="sw_err"></div>
        </div>
        <!-- step 2 -->
        <div data-step="2" class="d-none">
          <div id="sw_summary" class="mb-2"></div>
          <div class="d-flex flex-wrap gap-2 align-items-center mb-2">
            <input class="form-control form-control-sm" style="max-width:260px" id="sw_filter" placeholder="Filter by path, method or tag…">
            <div class="form-check ms-1"><input class="form-check-input" type="checkbox" id="sw_all" checked><label class="form-check-label small" for="sw_all">Select all shown</label></div>
            <span class="ms-auto small text-muted" id="sw_count"></span>
          </div>
          <div class="table-responsive" style="max-height:48vh"><table class="table table-sm align-middle mb-0">
            <thead class="sticky-top bg-white"><tr><th></th><th>Method</th><th>Path</th><th>Tag</th><th>Summary</th><th>Auth</th><th>Status</th></tr></thead>
            <tbody id="sw_rows"></tbody></table></div>
          <div id="sw_removed" class="mt-3"></div>
          <div class="card mt-3"><div class="card-body py-3">
            <div class="d-flex flex-wrap gap-2 align-items-center">
              <b class="small">Endpoint details</b>
              <span class="small text-muted">Swagger has no contacts, database, business unit or Azure info. Copy them from an endpoint you've already filled in:</span>
              <select class="form-select form-select-sm" id="sw_tpl" style="max-width:340px"></select>
            </div>
            <div class="small text-muted mt-2" id="sw_tpl_info"></div>
            <details class="mt-2"><summary class="small">Review / edit the details applied to every imported endpoint</summary>
              <div class="row g-2 mt-1" id="sw_details"></div></details>
            <div class="form-check mt-2"><input class="form-check-input" type="checkbox" id="sw_savehost" checked>
              <label class="form-check-label small" for="sw_savehost">Save these details on the host as its defaults (fills blanks only)</label></div>
          </div></div>
        </div>
        <!-- step 3 -->
        <div data-step="3" class="d-none">
          <div class="progress mb-3" role="progressbar"><div class="progress-bar" id="sw_bar" style="width:0%"></div></div>
          <div id="sw_result"></div>
        </div>
      </div>
      <div class="modal-footer">
        <button class="btn btn-outline-secondary me-auto d-none" id="sw_back" type="button">Back</button>
        <button class="btn btn-secondary" data-bs-dismiss="modal" type="button" id="sw_close">Cancel</button>
        <button class="btn btn-primary" id="sw_next" type="button">Fetch endpoints</button>
      </div>
    </div></div></div>`);
    modal = document.getElementById("swaggerModal");
    modal.querySelector("#sw_show").onclick = () => { const i = modal.querySelector("#sw_pass"); i.type = i.type === "password" ? "text" : "password"; };
    modal.querySelector("#sw_file").onchange = async e => { const f = e.target.files[0]; if (f) modal.querySelector("#sw_paste").value = await f.text(); };
    return modal;
  }

  const methodBadge = m => `<span class="badge method-${m.toLowerCase()}">${m}</span>`;

  // host: an existing Apihost to re-sync, or null for a new API. Resolves true if anything was imported.
  function open(host = null) {
    const m = ensureModal();
    const $ = s => m.querySelector(s);
    const bs = bootstrap.Modal.getOrCreateInstance(m);
    let parsed = null, existing = [], allHosts = [], targetHost = host, changed = false;

    const step = n => {
      m.querySelectorAll("[data-step]").forEach(d => d.classList.toggle("d-none", d.dataset.step !== String(n)));
      $("#sw_back").classList.toggle("d-none", n !== 2);
      $("#sw_next").classList.toggle("d-none", n === 3);
      $("#sw_close").textContent = n === 3 ? "Close" : "Cancel";
      $("#sw_next").textContent = "Fetch endpoints";
      $("#sw_next").disabled = false;
      if (n === 2) count();
    };
    const log = t => { $("#sw_log").textContent = t; };
    const err = t => { const e = $("#sw_err"); e.innerHTML = t; e.classList.toggle("d-none", !t); };

    // reset / prefill
    m.querySelector(".modal-title").textContent = host ? `Sync "${host.apiHostName || host.apiHostUrl}" from Swagger` : "Add API from Swagger";
    $("#sw_root").value = host?.apiHostUrl || ""; $("#sw_root").readOnly = !!host;
    $("#sw_name").value = host?.apiHostName || "";
    $("#sw_user").value = blank(host?.swaggerUsername) ? "" : host.swaggerUsername;
    $("#sw_pass").value = blank(host?.swaggerPassword) ? "" : host.swaggerPassword;
    $("#sw_paste").value = ""; $("#sw_file").value = ""; log(""); err("");
    step(1);

    async function doFetch() {
      err("");
      let root = $("#sw_root").value.trim();
      if (!/^https?:\/\//i.test(root)) { err("Enter the API URL, starting with https://"); return; }
      root = trimSlash(root);
      const user = $("#sw_user").value.trim(), pass = $("#sw_pass").value;
      $("#sw_next").disabled = true;
      try {
        let spec, url;
        const pasted = $("#sw_paste").value.trim();
        if (pasted) { spec = JSON.parse(pasted); url = root + "/swagger.json"; log("Using the pasted swagger.json."); }
        else ({ spec, url } = await findSpec(root, $("#sw_path").value.trim(), user, pass, log));
        parsed = parseSpec(spec, url);
        if (!parsed.origin || pasted) { const r = new URL(root); parsed.origin = r.origin; if (!parsed.basePath) parsed.basePath = trimSlash(r.pathname); parsed.rootUrl = trimSlash(parsed.origin + parsed.basePath); }
        log(`Loaded ${url}`);
      } catch (e) {
        $("#sw_next").disabled = false; log("");
        err(esc(e.message) + (e.kind === "network" ? `<div class="mt-1">You can also open the swagger.json link in a new tab, save it, and upload it below.</div>` : ""));
        return;
      }
      // What's already registered?
      try {
        [allHosts, existing] = await Promise.all([api.list("apihosts"), api.list("apiaudit")]);
      } catch (e) { $("#sw_next").disabled = false; err("Couldn't load current hosts/endpoints: " + esc(e.message)); return; }
      targetHost = host || allHosts.find(h => trimSlash(h.apiHostUrl).toLowerCase() === parsed.origin.toLowerCase()) || null;
      renderPreview();
      step(2);
    }

    function renderPreview() {
      const mine = targetHost ? existing.filter(e => e.apiHostId === targetHost.id) : [];
      const have = new Set(mine.map(e => opKey(e.type || e.description?.split(" ")[0], e.url)));
      const specKeys = new Set(parsed.endpoints.map(o => opKey(o.method, o.path)));
      parsed.endpoints.forEach(o => { o.exists = have.has(opKey(o.method, o.path)); o.pick = !o.exists; });
      const nAuth = parsed.endpoints.filter(o => o.auth).length;
      const nNew = parsed.endpoints.filter(o => !o.exists).length;
      $("#sw_summary").innerHTML = `
        <div class="d-flex flex-wrap gap-3 align-items-baseline">
          <h6 class="mb-0">${esc(parsed.title || "API")} <span class="text-muted fw-normal">${esc(parsed.version)}</span></h6>
          <span class="small text-muted">${esc(parsed.specVersion ? "OpenAPI " + parsed.specVersion : "")} · ${esc(parsed.rootUrl)}</span>
        </div>
        <div class="small mt-1">${parsed.endpoints.length} operations · <b>${nNew} new</b> · ${parsed.endpoints.length - nNew} already registered ·
          ${nAuth ? `${nAuth} declare authentication` : '<span class="text-warning-emphasis">none declare authentication in Swagger</span>'}
          ${targetHost ? ` · host <b>#${targetHost.id} ${esc(targetHost.apiHostName || "")}</b>` : " · a new host will be created"}</div>`;
      const gone = mine.filter(e => !specKeys.has(opKey(e.type || e.description?.split(" ")[0], e.url)) && e.isActive !== false);
      $("#sw_removed").innerHTML = gone.length ? `
        <div class="alert alert-warning small mb-0"><div class="form-check">
          <input class="form-check-input" type="checkbox" id="sw_deact" checked>
          <label class="form-check-label" for="sw_deact"><b>${gone.length}</b> registered endpoint(s) for this host are no longer in Swagger. Mark them inactive:</label></div>
          <div class="mt-1">${gone.slice(0, 12).map(e => esc(e.description || e.url)).join(" · ")}${gone.length > 12 ? " …" : ""}</div></div>` : "";
      $("#sw_removed").dataset.ids = gone.map(e => e.id).join(",");
      // Template: default to the most recently modified endpoint on this host that has details filled in
      const richness = e => Object.keys(copyable(e)).length;
      const candidates = existing.filter(e => richness(e) > 3)
        .sort((a, b) => (b.apiHostId === targetHost?.id) - (a.apiHostId === targetHost?.id) || Cocky.dateMs(b.modifiedDate) - Cocky.dateMs(a.modifiedDate));
      $("#sw_tpl").innerHTML = '<option value="">Don\'t copy (leave details blank)</option>' + candidates.map(e =>
        `<option value="${e.id}">#${e.id} ${esc(e.description || e.url || "")}${e.apiHostId === targetHost?.id ? " (this API)" : ""} · ${richness(e)} fields</option>`).join("");
      if (candidates.length) $("#sw_tpl").value = String(candidates[0].id);
      $("#sw_tpl").onchange = drawDetails;
      drawDetails();
      drawRows();
    }

    function drawDetails() {
      const tpl = existing.find(e => String(e.id) === $("#sw_tpl").value);
      const d = detailsFor(targetHost, tpl);
      const fromHost = Object.keys(hostDefaults(targetHost)).length;
      $("#sw_tpl_info").textContent = Object.keys(d).length
        ? `${Object.keys(d).length} fields will be copied to each new endpoint${fromHost ? ` (${fromHost} from the host's defaults${tpl ? ", the rest from the endpoint" : ""})` : ""}. Swagger values (method, path, tag, auth) always win.`
        : "Imported endpoints will only have what Swagger provides. Fill in the host's defaults (Hosts → Edit) or pick an endpoint to copy from.";
      $("#sw_details").innerHTML = DETAIL_FIELDS.map(k => `<div class="col-md-4 col-lg-3">
        <label class="form-label small mb-0 text-muted" for="swd_${k}">${esc(Cocky.titleCase(k))}</label>
        <input class="form-control form-control-sm" id="swd_${k}" data-dk="${k}" value="${esc(d[k] ?? "")}"></div>`).join("");
    }
    function details() {
      const tpl = existing.find(e => String(e.id) === $("#sw_tpl").value);
      const d = detailsFor(targetHost, tpl);
      m.querySelectorAll("[data-dk]").forEach(el => {
        const k = el.dataset.dk, v = el.value.trim();
        if (v === "") delete d[k];
        else d[k] = Cocky.SCHEMA.apiaudit.fields[k] === "number" ? Number(v) || 0 : v;
      });
      return d;
    }

    function drawRows() {
      const q = $("#sw_filter").value.trim().toLowerCase();
      const shown = parsed.endpoints.filter(o => !q || `${o.method} ${o.path} ${o.tag} ${o.summary}`.toLowerCase().includes(q));
      $("#sw_rows").innerHTML = shown.map(o => {
        const i = parsed.endpoints.indexOf(o);
        return `<tr class="${o.exists ? "text-muted" : ""}">
          <td><input class="form-check-input sw-pick" type="checkbox" data-i="${i}" ${o.pick ? "checked" : ""} ${o.exists ? "disabled" : ""}></td>
          <td>${methodBadge(o.method)}</td><td class="font-monospace small">${esc(o.path)}</td>
          <td class="small">${esc(o.tag)}</td><td class="small">${esc(o.summary).slice(0, 80)}</td>
          <td class="small">${o.auth ? `<span class="badge text-bg-success" title="${esc(o.authType)}">Required</span>` : '<span class="badge text-bg-light border">None</span>'}</td>
          <td class="small">${o.exists ? "Registered" : '<span class="text-success">New</span>'}${o.deprecated ? ' <span class="badge text-bg-secondary">deprecated</span>' : ""}</td></tr>`;
      }).join("") || '<tr><td colspan="7" class="text-center text-muted py-3">Nothing matches.</td></tr>';
      m.querySelectorAll(".sw-pick").forEach(cb => cb.onchange = () => { parsed.endpoints[+cb.dataset.i].pick = cb.checked; count(); });
      $("#sw_all").checked = shown.filter(o => !o.exists).every(o => o.pick);
      $("#sw_all").onchange = () => { shown.forEach(o => { if (!o.exists) o.pick = $("#sw_all").checked; }); drawRows(); };
      count();
    }
    const count = () => {
      const n = parsed.endpoints.filter(o => o.pick && !o.exists).length;
      $("#sw_count").textContent = `${n} selected`;
      $("#sw_next").textContent = `Import ${n} endpoint${n === 1 ? "" : "s"}`;
    };
    $("#sw_filter").oninput = drawRows;

    async function doImport() {
      const picks = parsed.endpoints.filter(o => o.pick && !o.exists);
      const deactIds = $("#sw_deact")?.checked ? ($("#sw_removed").dataset.ids || "").split(",").filter(Boolean).map(Number) : [];
      if (!picks.length && !deactIds.length) { toast("Nothing selected to import.", "warning"); return; }
      step(3);
      const bar = $("#sw_bar"), out = $("#sw_result");
      const total = picks.length + deactIds.length + 1;
      let done = 0; const tick = () => { bar.style.width = `${Math.round(++done / total * 100)}%`; };
      const failures = [], filled = new Set();
      const user = $("#sw_user").value.trim(), pass = $("#sw_pass").value, save = $("#sw_save").checked;
      const name = $("#sw_name").value.trim() || parsed.title || new URL(parsed.origin).hostname;

      // 1. host
      out.innerHTML = Cocky.spinner(targetHost ? "Updating host…" : "Creating host…");
      const now = new Date().toISOString();
      try {
        if (targetHost) {
          const upd = { ...targetHost, apiHostName: targetHost.apiHostName || name, isSecure: parsed.origin.startsWith("https:") };
          if (save) Object.assign(upd, { swaggerUsername: user || null, swaggerPassword: pass || null });
          await api.update("apihosts", targetHost.id, upd); targetHost = upd;
        } else {
          const body = { apiHostName: name, apiHostUrl: parsed.origin, swaggerUsername: save ? user || null : null, swaggerPassword: save ? pass || null : null,
            isSecure: parsed.origin.startsWith("https:"), lastAuditDate: null, lastAuditId: 0, active: true, createdDate: now };
          const created = await createWithRetry("apihosts", body, filled);
          targetHost = created && created.id != null ? created
            : (await api.list("apihosts")).find(h => trimSlash(h.apiHostUrl).toLowerCase() === parsed.origin.toLowerCase());
          if (!targetHost) throw new Error("The host was created but its id couldn't be found.");
        }
        changed = true;
      } catch (e) { out.innerHTML = `<div class="alert alert-danger">Couldn't save the host: ${esc(e.message)}</div>`; return; }
      tick();

      // 2. endpoints (a few at a time), with the shared details copied in
      const det = details();
      if ($("#sw_savehost").checked) {
        const hostFields = Cocky.SCHEMA.apihosts.fields, fill = {};
        for (const [k, v] of Object.entries(det)) {
          if (!(k in hostFields) || HOST_ONLY.has(k)) continue;
          const cur = targetHost[k];
          if (blank(cur) || (cur === 0 && ZERO_IS_UNSET.test(k)) || cur === false) fill[k] = v;
        }
        if (Object.keys(fill).length) {
          try { const upd = { ...targetHost, ...fill }; await api.update("apihosts", targetHost.id, upd); targetHost = upd; }
          catch (e) { failures.push("Saving details on the host: " + e.message); }
        }
      }
      let created = 0, i = 0;
      const worker = async () => {
        while (i < picks.length) {
          const o = picks[i++];
          out.innerHTML = Cocky.spinner(`Importing ${created + failures.length + 1} of ${picks.length}: ${o.method} ${o.path}`);
          try { await createWithRetry("apiaudit", endpointRecord(o, parsed, targetHost, det), filled); created++; }
          catch (e) { failures.push(`${o.method} ${o.path}: ${e.message}`); }
          tick();
        }
      };
      await Promise.all(Array.from({ length: 4 }, worker));

      // 3. deactivate removed
      let deactivated = 0;
      for (const id of deactIds) {
        const e = existing.find(x => x.id === id);
        try { await api.update("apiaudit", id, { ...e, isActive: false, modifiedDate: new Date().toISOString(), notes: [e.notes, "Removed from Swagger " + new Date().toLocaleDateString()].filter(Boolean).join(" · ") }); deactivated++; }
        catch (err2) { failures.push(`Deactivate #${id}: ${err2.message}`); }
        tick();
      }
      if (created || deactivated) changed = true;
      bar.style.width = "100%";
      out.innerHTML = `
        <div class="alert alert-${failures.length ? "warning" : "success"}">
          <b>${created}</b> endpoint(s) imported${deactivated ? `, <b>${deactivated}</b> marked inactive` : ""} for host <b>#${targetHost.id} ${esc(targetHost.apiHostName || "")}</b>.
          ${failures.length ? `<div class="mt-1"><b>${failures.length}</b> failed:</div><ul class="small mb-0">${failures.slice(0, 20).map(f => `<li>${esc(f)}</li>`).join("")}</ul>` : ""}
          ${filled.size ? `<div class="small mt-2">The API required these fields, so they were filled with "N/A" or 0: ${[...filled].map(esc).join(", ")}. Consider making them nullable.</div>` : ""}
        </div>
        <a class="btn btn-primary" href="apiaudit.html?host=${targetHost.id}">Audit this API now</a>
        <a class="btn btn-outline-secondary ms-2" href="apiendpoints.html">View endpoints</a>`;
    }

    $("#sw_next").onclick = () => $("[data-step='1']").classList.contains("d-none") ? doImport() : doFetch();
    $("#sw_back").onclick = () => step(1);
    return new Promise(resolve => {
      m.addEventListener("hidden.bs.modal", () => resolve(changed), { once: true });
      bs.show();
      setTimeout(() => (host ? $("#sw_user") : $("#sw_root")).focus(), 300);
    });
  }

  // ---------- one-click: walk a host's swagger.json ----------
  // Best template for details: the most recently edited, filled-in endpoint on the same host (else any host).
  function pickTemplate(existing, hostId) {
    const richness = e => Object.keys(copyable(e)).length;
    return existing.filter(e => richness(e) > 3)
      .sort((a, b) => (b.apiHostId === hostId) - (a.apiHostId === hostId) || Cocky.dateMs(b.modifiedDate) - Cocky.dateMs(a.modifiedDate))[0] || null;
  }

  // Small dialog asking for the Swagger login. Resolves { user, pass, save } or null if cancelled.
  function promptCredentials(host, message = "", lastUser = "") {
    let el = document.getElementById("credModal");
    if (!el) {
      document.body.insertAdjacentHTML("beforeend", `
      <div class="modal fade" id="credModal" tabindex="-1"><div class="modal-dialog"><div class="modal-content">
        <form>
        <div class="modal-header"><h5 class="modal-title">Swagger login</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
        <div class="modal-body">
          <p class="small text-muted mb-2" id="cr_host"></p>
          <div class="alert alert-warning small py-2 d-none" id="cr_msg"></div>
          <label class="form-label small mb-1" for="cr_user">Username</label><input class="form-control mb-2" id="cr_user" autocomplete="off">
          <label class="form-label small mb-1" for="cr_pass">Password</label><input class="form-control mb-2" id="cr_pass" type="password" autocomplete="new-password">
          <div class="form-check"><input class="form-check-input" type="checkbox" id="cr_save" checked><label class="form-check-label small" for="cr_save">Save on the host for next time</label></div>
        </div>
        <div class="modal-footer"><button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button><button type="submit" class="btn btn-primary">Continue</button></div>
        </form>
      </div></div></div>`);
      el = document.getElementById("credModal");
    }
    el.querySelector("#cr_host").textContent = `${host.apiHostName || ""} · ${host.apiHostUrl}`;
    const msg = el.querySelector("#cr_msg"); msg.textContent = message; msg.classList.toggle("d-none", !message);
    el.querySelector("#cr_user").value = lastUser || (blank(host.swaggerUsername) ? "" : host.swaggerUsername);
    el.querySelector("#cr_pass").value = "";
    // Bootstrap ignores show()/hide() while the dialog is still animating, so wait for each transition to finish.
    return credIdle.then(() => new Promise(resolve => {
      let result = null, shown = false, submitted = false;
      const bs = bootstrap.Modal.getOrCreateInstance(el);
      const close = () => { credIdle = new Promise(r => el.addEventListener("hidden.bs.modal", r, { once: true })); bs.hide(); };
      el.querySelector("form").onsubmit = ev => {
        ev.preventDefault();
        if (submitted) return;
        submitted = true;
        result = { user: el.querySelector("#cr_user").value.trim(), pass: el.querySelector("#cr_pass").value, save: el.querySelector("#cr_save").checked };
        if (shown) close(); else el.addEventListener("shown.bs.modal", close, { once: true });
      };
      el.addEventListener("hidden.bs.modal", () => resolve(result), { once: true });
      el.addEventListener("shown.bs.modal", () => { shown = true; el.querySelector(el.querySelector("#cr_user").value ? "#cr_pass" : "#cr_user").focus(); }, { once: true });
      bs.show();
    }));
  }
  let credIdle = Promise.resolve();

  // Fetch the host's swagger.json (asking for the login if needed), register any operations that
  // aren't endpoints yet (copying details from a template endpoint), and return what happened.
  // existing = all current Apiaudit records. log(text) reports progress.
  async function walkHost(host, existing, log = () => {}) {
    let user = blank(host.swaggerUsername) ? "" : host.swaggerUsername;
    let pass = blank(host.swaggerPassword) ? "" : host.swaggerPassword;
    let save = false, spec, url, askMsg = user && pass ? "" : null;
    for (let attempt = 0; ; attempt++) {
      if (askMsg !== "") {
        const c = await promptCredentials(host, askMsg || "", user);
        if (!c) throw Object.assign(new Error("Cancelled: no Swagger login entered."), { kind: "cancel" });
        ({ user, pass, save } = c);
      }
      try { ({ spec, url } = await findSpec(host.apiHostUrl, "/swagger/v1/swagger.json", user, pass, log)); break; }
      catch (e) {
        if (e.kind === "auth" && attempt < 2) { askMsg = e.message + " Try again."; continue; }
        throw e;
      }
    }
    if (save) {
      const upd = { ...host, swaggerUsername: user || null, swaggerPassword: pass || null };
      try { await api.update("apihosts", host.id, upd); Object.assign(host, upd); } catch { /* not fatal */ }
    }
    const parsed = parseSpec(spec, url);
    log(`Read ${parsed.endpoints.length} operations from ${url}`);
    const mine = existing.filter(e => e.apiHostId === host.id);
    const have = new Set(mine.map(e => opKey(e.type || String(e.description || "").split(" ")[0], e.url)));
    const specKeys = new Set(parsed.endpoints.map(o => opKey(o.method, o.path)));
    const missing = parsed.endpoints.filter(o => !have.has(opKey(o.method, o.path)));
    const notInSwagger = mine.filter(e => e.isActive !== false && !specKeys.has(opKey(e.type || String(e.description || "").split(" ")[0], e.url)));
    const tpl = pickTemplate(existing, host.id), det = detailsFor(host, tpl);
    const created = [], failures = [], filled = new Set();
    let i = 0;
    const worker = async () => {
      while (i < missing.length) {
        const o = missing[i++];
        log(`Registering ${created.length + failures.length + 1} of ${missing.length}: ${o.method} ${o.path}`);
        const body = endpointRecord(o, parsed, host, det);
        try {
          const r = await createWithRetry("apiaudit", body, filled);
          created.push(r && r.id != null ? r : body);
        } catch (e) { failures.push(`${o.method} ${o.path}: ${e.message}`); }
      }
    };
    await Promise.all(Array.from({ length: 4 }, worker));
    // Existing endpoints on this host: copy the host's filled-in details onto them (only fields that differ)
    const hd = hostDefaults(host), hdKeys = Object.keys(hd);
    const stale = hdKeys.length ? mine.filter(e => hdKeys.some(k => e[k] !== hd[k])) : [];
    let refreshed = 0, j = 0;
    const pusher = async () => {
      while (j < stale.length) {
        const e = stale[j++];
        log(`Updating details on existing endpoint ${j} of ${stale.length}`);
        try { await api.update("apiaudit", e.id, { ...e, ...hd, modifiedDate: new Date().toISOString() }); refreshed++; }
        catch (err) { failures.push(`Updating #${e.id}: ${err.message}`); }
      }
    };
    await Promise.all(Array.from({ length: 4 }, pusher));
    // Reload so every new endpoint has its id and updated ones show the new details.
    let endpoints = existing;
    if (created.length || refreshed) { try { endpoints = await api.list("apiaudit"); } catch { endpoints = existing.concat(created.filter(c => c.id != null)); } }
    return { parsed, specUrl: url, created, failures, filled: [...filled], notInSwagger, template: tpl, hostDefaultCount: hdKeys.length, refreshed, endpoints };
  }

  // Copy the host's filled-in defaults onto every endpoint of that host. Returns { updated, failures, fields }.
  async function pushHostDetails(host, onProgress = () => {}) {
    const d = hostDefaults(host);
    const fields = Object.keys(d);
    if (!fields.length) return { updated: 0, failures: [], fields };
    const mine = (await api.list("apiaudit")).filter(e => e.apiHostId === host.id);
    let updated = 0, i = 0; const failures = [];
    const worker = async () => {
      while (i < mine.length) {
        const e = mine[i++];
        const changed = fields.some(k => e[k] !== d[k]);
        if (changed) {
          try { await api.update("apiaudit", e.id, { ...e, ...d, modifiedDate: new Date().toISOString() }); updated++; }
          catch (err) { failures.push(`#${e.id}: ${err.message}`); }
        }
        onProgress(i, mine.length);
      }
    };
    await Promise.all(Array.from({ length: 4 }, worker));
    return { updated, total: mine.length, failures, fields };
  }

  window.SwaggerImport = { open, parseSpec, findSpec, walkHost, promptCredentials, pushHostDetails, hostDefaults };
})();
