// CockyAuditor back-end interfaces (build 5.0): discovery and endpoint association.
// Load after app.js (and swagger-import.js when the login prompt is wanted).
//
// Two steps:
//  1. Discovery, per host (API). While a host is being audited, CockyAuditor reads that API's own SystemConsole
//     (config.js -> interfaceDiscovery.paths), which lists the services/interfaces registered behind it, and saves
//     one ApiInterfacesAudit row per interface method with apiHostId = the host and apiAuditId empty.
//     Rows are matched on host + interface + method, so re-auditing never duplicates them.
//  2. Association, later. On the Interfaces page each method is tied to one of that host's endpoints (apiAuditId),
//     with suggestions from the names (IPaymentService24.GetById24 -> GET /api/Payments/{id}).
// Discovery only: the auditor records what's exposed and never calls the interface methods.
(function () {
  const { api, esc, CFG } = window.Cocky;
  const DISC = CFG.interfaceDiscovery || {};
  const PATHS = DISC.paths || ["/SystemConsole", "/api/SystemConsole"];
  const R = "apiinterfaces";
  const blank = v => v == null || String(v).trim() === "" || String(v).trim().toLowerCase() === "string";
  const joinUrl = (base, path) => /^https?:\/\//i.test(path) ? path
    : String(base || "").replace(/\/+$/, "") + "/" + String(path || "").replace(/^\/+/, "");
  const MUTATING = /^(create|add|insert|post|save|update|put|patch|edit|modify|delete|remove)/i;
  const isMutating = m => MUTATING.test(String(m || ""));
  const keyOf = (i, m) => `${String(i ?? "").trim().toLowerCase()}::${String(m ?? "").trim().toLowerCase()}`;
  const METHODS = ["GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS"];

  // ---------- endpoints ----------
  const endpointMethod = e => { const m = String(e.type || e.endpointType || String(e.description || "").split(" ")[0] || "").toUpperCase(); return METHODS.includes(m) ? m : "GET"; };
  const endpointPath = e => Cocky.e9.pathOf(e) || "";
  const endpointLabel = e => `${endpointMethod(e)} ${endpointPath(e) || e.description || "(no path)"}`;

  // The host an interface row belongs to: its own apiHostId, or (rows saved before 5.0) its endpoint's host
  function hostOf(row, endpointsById) {
    if (row.apiHostId != null && row.apiHostId !== 0) return row.apiHostId;
    const e = row.apiAuditId != null ? endpointsById.get(row.apiAuditId) : null;
    return e ? e.apiHostId ?? null : null;
  }
  const byId = rows => new Map((rows || []).map(r => [r.id, r]));
  const isAssociated = r => r.apiAuditId != null && r.apiAuditId !== 0;

  // ---------- parsing the SystemConsole answer ----------
  // Accepts [{ interfaceName, methodCount, methods: ["Create02", ...] }], a single object, or a wrapped list
  // ({ items | data | value | interfaces | services: [...] }). Methods may be strings or { name | methodName }.
  // Also picks up implementation / lifetime when the console reports them. Returns unique method rows.
  function field(o, names) {
    for (const n of names) {
      const k = Object.keys(o).find(x => x.toLowerCase() === n.toLowerCase());
      if (k !== undefined && !blank(o[k])) return o[k];
    }
    return null;
  }
  function parse(data) {
    if (typeof data === "string") data = JSON.parse(data);
    const list = Array.isArray(data) ? data
      : data && typeof data === "object" ? (["items", "data", "value", "interfaces", "services", "results"].map(k => field(data, [k])).find(Array.isArray) || [data])
      : [];
    const rows = [], seen = new Set();
    for (const it of list) {
      if (!it || typeof it !== "object") continue;
      const name = field(it, ["interfaceName", "interface", "serviceType", "service", "name"]);
      if (blank(name)) continue;
      const impl = field(it, ["implementationName", "implementation", "implementationType", "implementedBy", "className"]);
      const life = field(it, ["serviceLifetime", "lifetime", "scope"]);
      const implemented = field(it, ["isImplemented", "implemented"]);
      const methods = field(it, ["methods", "methodNames", "operations"]);
      const names = Array.isArray(methods) && methods.length
        ? methods.map(m => typeof m === "string" ? m : m && typeof m === "object" ? field(m, ["name", "methodName"]) : m).filter(m => !blank(m)).map(String)
        : [null];
      for (const m of names) {
        const k = keyOf(name, m);
        if (seen.has(k)) continue;
        seen.add(k);
        rows.push({ interfaceName: String(name).trim(), methodName: m == null ? null : m.trim(),
          implementationName: impl == null ? null : String(impl), serviceLifetime: life == null ? null : String(life),
          isImplemented: implemented == null ? true : implemented === true || /^(true|1|yes)$/i.test(String(implemented)) });
      }
    }
    return rows;
  }

  // ---------- fetching it from the host ----------
  // GET <apiHostUrl><path> for each configured path, with the host's Swagger login as Basic auth when there is one.
  // Resolves { url, rows }. Errors carry kind: "missing" (no path answered), "auth", "network" (usually CORS), "format".
  async function fetchConsole(host, creds = {}, log = () => {}) {
    if (blank(host.apiHostUrl)) throw Object.assign(new Error("The host has no API Host Url."), { kind: "missing" });
    const user = creds.user ?? (blank(host.swaggerUsername) ? "" : host.swaggerUsername);
    const pass = creds.pass ?? (blank(host.swaggerPassword) ? "" : host.swaggerPassword);
    const headers = { Accept: "application/json" };
    if (user || pass) headers.Authorization = "Basic " + btoa(unescape(encodeURIComponent(`${user}:${pass}`)));
    const tried = [];
    for (const p of PATHS) {
      const url = joinUrl(host.apiHostUrl, p);
      tried.push(url);
      log(`Reading interfaces from ${url}…`);
      const ctrl = new AbortController();
      const t = setTimeout(() => ctrl.abort(), CFG.requestTimeoutMs);
      let res;
      try { res = await fetch(url, { headers, signal: ctrl.signal, cache: "no-store" }); }
      catch (e) {
        throw Object.assign(new Error(e.name === "AbortError" ? `${url} didn't answer within ${CFG.requestTimeoutMs / 1000}s.`
          : `The browser couldn't read ${url}. The API has to allow this page in CORS (including the Authorization header).`), { kind: "network", url });
      } finally { clearTimeout(t); }
      if (res.status === 404 || res.status === 405) continue;
      if (res.status === 401 || res.status === 403) throw Object.assign(new Error(`${url} answered ${res.status}: the login was refused.`), { kind: "auth", url });
      if (!res.ok) throw Object.assign(new Error(`${url} answered HTTP ${res.status}.`), { kind: "http", url, status: res.status });
      const text = await res.text();
      let data;
      try { data = JSON.parse(text); } catch { throw Object.assign(new Error(`${url} didn't return JSON.`), { kind: "format", url }); }
      return { url, rows: parse(data) };
    }
    throw Object.assign(new Error(`No SystemConsole on this host (tried ${tried.join(", ")}).`), { kind: "missing", tried });
  }

  // ---------- comparing with what's recorded ----------
  // found = parse() rows; existing = every ApiInterfacesAudit row; endpoints = every Apiaudit row.
  // Returns { mine, toCreate, toRetire, toRevive, matched, dupes }.
  function reconcile(found, hostId, existing, endpoints) {
    const eps = byId(endpoints);
    const mine = (existing || []).filter(r => hostOf(r, eps) === hostId);
    const byKey = new Map(), dupes = [];
    for (const r of mine) { const k = keyOf(r.interfaceName, r.methodName); if (byKey.has(k)) dupes.push(r); else byKey.set(k, r); }
    const foundKeys = new Set(found.map(f => keyOf(f.interfaceName, f.methodName)));
    const toCreate = found.filter(f => !byKey.has(keyOf(f.interfaceName, f.methodName)));
    const toRevive = [...byKey.entries()].filter(([k, r]) => foundKeys.has(k) && r.isRegistered === false).map(([, r]) => r);
    const toRetire = [...byKey.entries()].filter(([k, r]) => !foundKeys.has(k) && r.isRegistered !== false).map(([, r]) => r);
    return { mine, toCreate, toRetire, toRevive, matched: found.length - toCreate.length, dupes };
  }

  // Can the API store apiHostId yet? Unknown until there's a row to look at.
  const hostColumnMissing = existing => existing.length > 0 && !existing.some(r => "apiHostId" in r);
  const SETUP = `ApiInterfacesAudit has no ApiHostId column yet, so interfaces can't be saved against a host. ` +
    `Run db/ApiInterfacesAudit-5.0.sql and add "public int? ApiHostId" (and make ApiAuditId "int?") on the model.`;

  const NULLABLE = `The API still requires ApiAuditId, so a method can't be saved before it's associated with an endpoint. ` +
    `Make it "public int? ApiAuditId" on the model and run db/ApiInterfacesAudit-5.0.sql (ALTER COLUMN ApiAuditId INT NULL).`;

  // Rows saved by the old (4.8) paste import: no host, and tied to whichever endpoint was picked in that dialog.
  const legacyRows = (rows, endpointsById) => (rows || []).filter(r => (r.apiHostId == null || r.apiHostId === 0) && isAssociated(r) && endpointsById.get(r.apiAuditId));
  // Move them to their endpoint's host and clear the endpoint, so they can be associated properly.
  // If the API still requires ApiAuditId, the host is set and the endpoint kept. Resolves { moved, keptEndpoint, failures }.
  async function moveLegacy(rows, endpointsById, log = () => {}) {
    let moved = 0, keptEndpoint = 0; const failures = [];
    for (const r of rows) {
      const e = endpointsById.get(r.apiAuditId);
      log(`Moving ${moved + keptEndpoint + failures.length + 1} of ${rows.length}…`);
      const body = { ...r, apiHostId: e.apiHostId, apiAuditId: null, route: null, httpMethod: null, controllerName: null };
      try { await api.update(R, r.id, body); Object.assign(r, body); moved++; }
      catch (err) {
        if (err.status !== 400) { failures.push(`#${r.id}: ${err.message}`); continue; }
        try { const b2 = { ...r, apiHostId: e.apiHostId }; await api.update(R, r.id, b2); Object.assign(r, b2); keptEndpoint++; }
        catch (err2) { failures.push(`#${r.id}: ${err2.message}`); }
      }
    }
    return { moved, keptEndpoint, failures };
  }

  // Write the plan. Resolves { created, retired, revived, failures }.
  async function apply(plan, hostId, log = () => {}, host = null) {
    const now = new Date().toISOString();
    const created = [], failures = [];
    let retired = 0, revived = 0;
    const run = async (items, fn) => { let i = 0; await Promise.all(Array.from({ length: 4 }, async () => { while (i < items.length) await fn(items[i++]); })); };
    let checked = false;
    const create = async f => {
      // endpointName = the API (host) name, as on the rows the API already has; apiAuditId stays empty until associated
      const body = { apiHostId: hostId, apiAuditId: null, endpointName: host ? (host.apiHostName || host.apiHostUrl || null) : null,
        interfaceName: f.interfaceName, methodName: f.methodName,
        implementationName: f.implementationName, serviceLifetime: f.serviceLifetime, isImplemented: f.isImplemented !== false,
        controllerName: null, route: null, httpMethod: null, isRegistered: true, discoveredDate: now };
      try {
        const saved = await api.create(R, body);
        // First row saved: make sure the API kept apiHostId (an older model silently drops it)
        if (!checked && saved && typeof saved === "object") {
          checked = true;
          if (!("apiHostId" in saved)) { failures.push(SETUP); throw Object.assign(new Error(SETUP), { kind: "setup", saved }); }
        }
        created.push(saved && typeof saved === "object" ? saved : body);
      } catch (e) {
        if (e.kind === "setup") throw e;
        failures.push(`${f.interfaceName}.${f.methodName || "(no methods)"}: ${e.status === 400 && /apiAuditId/i.test(e.message) ? NULLABLE : e.message}`);
      }
    };
    // Create one on its own first so a missing column is caught before writing the rest
    const first = plan.toCreate.slice(0, 1), rest = plan.toCreate.slice(1);
    if (first.length) { log(`Saving interface method 1 of ${plan.toCreate.length}…`); await create(first[0]); }
    await run(rest, async f => { log(`Saving interface method ${created.length + failures.length + 1} of ${plan.toCreate.length}…`); await create(f); });
    await run(plan.toRevive, async r => { try { await api.update(R, r.id, { ...r, isRegistered: true }); r.isRegistered = true; revived++; } catch (e) { failures.push(`#${r.id}: ${e.message}`); } });
    await run(plan.toRetire, async r => { try { await api.update(R, r.id, { ...r, isRegistered: false }); r.isRegistered = false; retired++; } catch (e) { failures.push(`#${r.id}: ${e.message}`); } });
    return { created, retired, revived, failures };
  }

  // ---------- confirmation (same idea as new endpoints on Audit host) ----------
  const methodBadge = m => `<span class="badge method-${String(m).toLowerCase()} me-1">${esc(m)}</span>`;
  function planSummaryHtml(plan, found) {
    const tile = (n, label, cls = "") => `<div class="col"><div class="border rounded py-2"><div class="fs-3 fw-bold ${cls}">${n}</div><div class="small text-muted">${label}</div></div></div>`;
    const groups = new Map();
    plan.toCreate.forEach(f => (groups.get(f.interfaceName) || groups.set(f.interfaceName, []).get(f.interfaceName)).push(f));
    return `<div class="row g-2 text-center mb-3">
        ${tile(plan.toCreate.length, "new methods to save", plan.toCreate.length ? "text-success" : "")}
        ${tile(plan.matched, "already recorded")}
        ${tile(plan.toRetire.length, "no longer registered", plan.toRetire.length ? "text-danger" : "")}
        ${tile(plan.toRevive.length, "registered again")}
      </div>
      ${groups.size ? `<h6 class="small text-uppercase text-muted">Will be saved (${new Set(found.map(f => f.interfaceName.toLowerCase())).size} interfaces reported)</h6>
      <div class="table-responsive" style="max-height:260px"><table class="table table-sm mb-2"><tbody>
        ${[...groups.entries()].map(([name, ms]) => `<tr><td class="font-monospace small text-nowrap">${esc(name)}</td><td class="small">${ms.map(m =>
          `<code>${esc(m.methodName || "(no methods listed)")}</code>${isMutating(m.methodName) ? ' <span class="badge text-bg-warning">writes</span>' : ""}`).join(", ")}</td></tr>`).join("")}
      </tbody></table></div>` : ""}
      ${plan.toRetire.length ? `<div class="small text-muted">No longer reported, will be marked not registered: ${plan.toRetire.slice(0, 8).map(r => `<code>${esc(r.interfaceName)}.${esc(r.methodName || "")}</code>`).join(", ")}${plan.toRetire.length > 8 ? "…" : ""}</div>` : ""}
      ${plan.dupes.length ? `<div class="small text-warning mt-1">${plan.dupes.length} recorded row(s) are duplicates of another on this host; they're left alone.</div>` : ""}`;
  }
  // Resolves "save", "skip" or null (closed)
  function confirmPlan(host, plan, found, url) {
    let el = document.getElementById("ifcModal");
    if (!el) {
      document.body.insertAdjacentHTML("beforeend", `
      <div class="modal fade" id="ifcModal" tabindex="-1" data-bs-backdrop="static"><div class="modal-dialog modal-lg modal-dialog-scrollable"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">Save back-end interfaces?</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
        <div class="modal-body"><p class="mb-2" id="ifc_sum"></p><div id="ifc_body"></div></div>
        <div class="modal-footer">
          <button type="button" class="btn btn-outline-secondary me-auto" data-choice="skip">Skip interfaces</button>
          <button type="button" class="btn btn-primary" data-choice="save" id="ifc_save"></button>
        </div>
      </div></div></div>`);
      el = document.getElementById("ifcModal");
    }
    el.querySelector("#ifc_sum").innerHTML = `<b>${esc(host.apiHostName || host.apiHostUrl)}</b>'s SystemConsole (<code>${esc(url)}</code>) reports
      <b>${found.length}</b> interface method(s). They're saved against the host; tie them to endpoints later on the Interfaces page.`;
    el.querySelector("#ifc_body").innerHTML = planSummaryHtml(plan, found);
    const n = plan.toCreate.length, u = plan.toRetire.length + plan.toRevive.length;
    el.querySelector("#ifc_save").textContent = n ? `Save ${n} method${n === 1 ? "" : "s"}${u ? ` & update ${u}` : ""}` : `Update ${u}`;
    return new Promise(resolve => {
      let result = null;
      const bs = bootstrap.Modal.getOrCreateInstance(el);
      el.querySelectorAll("[data-choice]").forEach(b => b.onclick = () => { result = b.dataset.choice; bs.hide(); });
      el.addEventListener("hidden.bs.modal", () => resolve(result), { once: true });
      bs.show();
    });
  }

  // ---------- one call used by Audit host ----------
  // Never throws: the audit goes on whatever happens here. Resolves a summary
  // { status: "saved" | "unchanged" | "skipped" | "none" | "setup" | "error", message, url, found, created, retired, revived, unassociated, failures }.
  async function discoverHost(host, { creds, log = () => {}, confirm = true, endpoints } = {}) {
    const out = { status: "error", host, found: 0, created: 0, retired: 0, revived: 0, unassociated: 0, failures: [] };
    let got;
    try { got = await fetchConsole(host, creds, log); }
    catch (e) { out.status = e.kind === "missing" ? "none" : "error"; out.message = e.message; out.kind = e.kind; return out; }
    out.url = got.url; out.found = got.rows.length;
    let existing;
    try {
      existing = await api.list(R);
      if (!endpoints) endpoints = await api.list("apiaudit");
    } catch (e) { out.message = e.status === 404 ? `The interfaces route ${CFG.endpoints[R]} isn't deployed.` : e.message; out.status = e.status === 404 ? "setup" : "error"; return out; }
    if (hostColumnMissing(existing)) { out.status = "setup"; out.message = SETUP; return out; }
    const plan = reconcile(got.rows, host.id, existing, endpoints);
    const unassoc = () => plan.mine.filter(r => !isAssociated(r) && r.isRegistered !== false).length;
    if (!plan.toCreate.length && !plan.toRetire.length && !plan.toRevive.length) {
      out.status = "unchanged"; out.unassociated = unassoc(); return out;
    }
    if (confirm) {
      const c = await confirmPlan(host, plan, got.rows, got.url);
      if (c !== "save") { out.status = "skipped"; out.message = "Interfaces weren't saved (skipped)."; out.pending = plan.toCreate.length; return out; }
    }
    try {
      const r = await apply(plan, host.id, log, host);
      Object.assign(out, { created: r.created.length, retired: r.retired, revived: r.revived, failures: r.failures, status: "saved" });
      out.unassociated = unassoc() + r.created.length;
    } catch (e) { out.status = e.kind === "setup" ? "setup" : "error"; out.message = e.message; }
    return out;
  }

  // One-line HTML summary for the Run Audit banner
  function summaryHtml(s) {
    const link = `apiinterfaces.html?host=${encodeURIComponent(s.host.id)}`;
    const assoc = s.unassociated ? ` <b>${s.unassociated}</b> not yet associated with an endpoint: <a href="${link}&assoc=1">Associate</a>.` : ` <a href="${link}">View interfaces</a>.`;
    switch (s.status) {
      case "saved": return `Interfaces: ${s.found} method(s) reported by <code>${esc(s.url)}</code>. <b>${s.created}</b> new saved` +
        `${s.retired ? `, ${s.retired} no longer registered` : ""}${s.revived ? `, ${s.revived} registered again` : ""}.` + assoc +
        (s.failures.length ? ` <span class="text-danger">${s.failures.length} failed: ${esc(s.failures[0])}</span>` : "");
      case "unchanged": return `Interfaces: ${s.found} method(s) reported, all already recorded.` + assoc;
      case "skipped": return `Interfaces: ${s.found} method(s) reported; ${s.pending || 0} new ones weren't saved (skipped). <a href="${link}&import=1">Import later</a>.`;
      case "none": return `Interfaces: this host has no SystemConsole (${esc(s.message)}). Paths are set in config.js → interfaceDiscovery.`;
      case "setup": return `<span class="text-danger">Interfaces: ${esc(s.message)}</span>`;
      default: return `Interfaces couldn't be read: ${esc(s.message || "unknown error")} <a href="${link}&import=1">Paste them instead</a>.`;
    }
  }

  // ---------- suggestions: which endpoint does a method serve? ----------
  // IPaymentService24 -> "payment"; IApilogService02 -> "apilog"; Enterprise.IUserHelpRepository -> "userhelp"
  function resourceOf(interfaceName) {
    let s = String(interfaceName || "").split(/[.+]/).pop().replace(/<.*$/, "").replace(/`\d+$/, "");
    s = s.replace(/^I(?=[A-Z])/, "").replace(/\d+$/, "").replace(/(Services?|Repository|Repo|Manager|Provider|Handler)$/i, "").replace(/\d+$/, "");
    return s.toLowerCase();
  }
  // GetById24 -> { verbs: [GET], param: true }
  function verbOf(methodName) {
    const m = String(methodName || "").replace(/Async$/i, "").replace(/\d+$/, "");
    if (/^(getall|list|getlist|findall|search|query)/i.test(m)) return { verbs: ["GET"], param: false, base: m };
    if (/^(get|find|fetch|read|load|retrieve)(by|one|single)/i.test(m)) return { verbs: ["GET"], param: true, base: m };
    if (/^(get|find|fetch|read|load|retrieve)/i.test(m)) return { verbs: ["GET"], param: null, base: m };
    if (/^(create|add|insert|post|save|new)/i.test(m)) return { verbs: ["POST"], param: false, base: m };
    if (/^patch/i.test(m)) return { verbs: ["PATCH", "PUT"], param: true, base: m };
    if (/^(update|put|edit|modify|replace)/i.test(m)) return { verbs: ["PUT", "PATCH"], param: true, base: m };
    if (/^(delete|remove|destroy)/i.test(m)) return { verbs: ["DELETE"], param: true, base: m };
    return { verbs: [], param: null, base: m };
  }
  const plural = s => [s, s + "s", s + "es", s.replace(/y$/, "ies"), s.replace(/e?s$/, "")];
  // Best endpoint of the host for this interface method, or null. Resolves { endpoint, score, why }.
  function suggest(row, hostEndpoints) {
    const res = resourceOf(row.interfaceName), v = verbOf(row.methodName);
    const names = new Set(res ? plural(res) : []);
    const opNorm = String(row.methodName || "").replace(/Async$/i, "").replace(/\d+$/, "").toLowerCase();
    let best = null, tie = false;
    for (const e of hostEndpoints) {
      const segs = Cocky.e9.segsOf(e).filter(s => !/^(api|v\d+)$/i.test(s));
      const words = segs.filter(s => !Cocky.e9.isParam(s)).map(s => s.toLowerCase());
      const hasParam = segs.some(Cocky.e9.isParam);
      const method = endpointMethod(e);
      let score = 0; const why = [];
      const op = String(e.swaggerOperationId || "").toLowerCase().replace(/\d+$/, "");
      // The resource must match: operationIds are often generic ("GetAll" on every controller), so they only break ties
      const resHit = words.some(w => names.has(w)) || (!blank(e.family) && names.has(String(e.family).toLowerCase()));
      if (!resHit) continue;
      score += 4; why.push("resource"); if (names.has(words[words.length - 1])) score += 1;
      if (op && opNorm && op === opNorm) { score += 3; why.push("operationId"); }
      if (v.verbs.length) {
        if (!v.verbs.includes(method)) continue;
        score += v.verbs[0] === method ? 3 : 2; why.push(method);
        if (v.param !== null) { if (v.param === hasParam) { score += 2; why.push(hasParam ? "{id}" : "no {id}"); } else score -= 2; }
        else if (!hasParam) score += 1;
      } else if (!why.includes("operationId")) continue;
      if (!best || score > best.score) { best = { endpoint: e, score, why }; tie = false; }
      else if (score === best.score) tie = true;
    }
    return best && !tie && best.score >= 7 ? best : null;
  }

  // Fields written when a method is tied to an endpoint (or untied: endpoint = null)
  function associationFields(row, endpoint, hostId) {
    return endpoint
      ? { apiAuditId: endpoint.id, apiHostId: row.apiHostId || endpoint.apiHostId || hostId || null, httpMethod: endpointMethod(endpoint),
          route: endpointPath(endpoint) || null, controllerName: blank(endpoint.family) ? row.controllerName ?? null : endpoint.family }
      : { apiAuditId: null, apiHostId: row.apiHostId || hostId || null, httpMethod: null, route: null, controllerName: null };
  }

  window.Interfaces = { PATHS, parse, fetchConsole, reconcile, apply, confirmPlan, planSummaryHtml, discoverHost, summaryHtml,
    suggest, resourceOf, verbOf, associationFields, hostOf, byId, isAssociated, isMutating, keyOf,
    endpointMethod, endpointPath, endpointLabel, hostColumnMissing, SETUP, NULLABLE, legacyRows, moveLegacy };
})();
