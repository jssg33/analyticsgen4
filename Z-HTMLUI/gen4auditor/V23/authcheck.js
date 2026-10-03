// Token / auth enforcement check (build 6.6), run by Run Audit on every endpoint. window.AuthCheck.
//
// Two compliance views:
//   STRICT    - if the whole world could reach the endpoint, would it be refused without a valid token or login?
//               Proven by calling it with no credentials and with deliberately invalid ones: both must be refused (401/403
//               or a redirect to sign-in) and, when credentials are given, valid ones must get through.
//   PRACTICAL - compliant when STRICT passes, OR the world can't reach it from a browser or the internet:
//               CORS restricted (no wildcard: only named origins such as your UI may call it) AND IP restricted (a private
//               address, or allowed IP ranges recorded on the endpoint / host). CORS alone is not enough: browsers enforce it,
//               but curl, scripts and servers ignore it, so the IP restriction is what keeps the world out.
//
// Credentials are kept in memory for this tab only (never saved): per host, with a per-endpoint override.
// Methods: auto | bearer (Authorization: Bearer) | pathToken ({token} in the route, like /api/weather/{token}) |
//          basic (username + password; WordPress application passwords) | apikey (a header) | none.
(function () {
  const CFG = window.COCKY_CONFIG;
  const AC = Object.assign({
    enabled: true, probeWrites: false, sampleValue: "1",
    tokenParams: "^(token|usertoken|user_token|sessiontoken|accesstoken|access_token|apitoken|authtoken|jwt)$",
    publicRoutes: ["/auth/(login|signin|signup|register|refresh)", "/(health|healthz|ping|status)$", "/swagger", "/openapi", "^/?$", "/wp-json/?$"],
    badToken: "cockyauditor-invalid-token-test", badUser: "cockyauditor-invalid", badPass: "invalid-password-test",
    apiKeyHeader: "X-Api-Key", timeoutMs: 15000
  }, CFG.authCheck || {});
  const TOKEN_RE = new RegExp(AC.tokenParams, "i");
  const METHODS = ["auto", "bearer", "pathToken", "basic", "apikey", "none"];
  const LABELS = { auto: "Auto (from the route / Auth Type)", bearer: "Bearer token (signed-in user's token)", pathToken: "Token in the path ({token})",
    basic: "Username + password (Basic / WordPress)", apikey: "API key header", none: "No credentials (only check it refuses anonymous calls)" };
  const val = v => (v == null || ["", "string", "n/a"].includes(String(v).trim().toLowerCase())) ? "" : String(v).trim();

  // ---------- credentials (memory only) ----------
  const creds = { host: new Map(), endpoint: new Map() };   // id -> { method, token, user, pass, header, key }
  const credsFor = e => creds.endpoint.get(e.id) || creds.host.get(e.apiHostId) || { method: "auto" };
  const hasOwn = e => creds.endpoint.has(e.id);

  function inferMethod(e, url) {
    const params = [...String(url).matchAll(/\{([^}]+)\}/g)].map(m => m[1].split(":")[0]);
    if (params.some(p => TOKEN_RE.test(p))) return "pathToken";
    const t = `${val(e.authType)} ${url}`;
    if (/basic|wordpress|application.?password|\/wp-json\//i.test(t)) return "basic";
    if (/api.?key/i.test(val(e.authType))) return "apikey";
    return "bearer";
  }
  const signedInToken = () => String(window.CockyAuth?.current()?.token || "").replace(/^Bearer\s+/i, "");

  // Build the request for one probe: kind = "anon" | "bad" | "good". Returns { url, headers } or null (can't build it).
  function build(method, c, url, kind) {
    const headers = { Accept: "application/json" };
    let token = kind === "good" ? (c.token || signedInToken()) : AC.badToken;
    let target = String(url).replace(/\{([^}]+)\}/g, (m, p) => {
      const name = p.split(":")[0];
      if (TOKEN_RE.test(name)) return method === "pathToken" ? encodeURIComponent(kind === "anon" ? "" : token) : encodeURIComponent(AC.sampleValue);
      return encodeURIComponent(AC.sampleValue);
    }).replace(/([^:])\/\/+/g, "$1/");
    if (kind === "anon" || method === "none" && kind !== "bad") return { url: target, headers };
    switch (method) {
      case "bearer": if (!token) return null; headers.Authorization = "Bearer " + token; break;
      case "pathToken": if (!token) return null; break;
      case "basic":
        if (kind === "good" && !(c.user && c.pass)) return null;
        headers.Authorization = "Basic " + btoa(unescape(encodeURIComponent(kind === "good" ? `${c.user}:${c.pass}` : `${AC.badUser}:${AC.badPass}`)));
        break;
      case "apikey":
        if (kind === "good" && !c.key) return null;
        headers[c.header || AC.apiKeyHeader] = kind === "good" ? c.key : AC.badToken; break;
      case "none": if (kind === "good") return null; headers.Authorization = "Bearer " + AC.badToken; break;
    }
    return { url: target, headers };
  }

  async function send(httpMethod, req, extra = {}) {
    const ctrl = new AbortController(), t = setTimeout(() => ctrl.abort(), AC.timeoutMs);
    try {
      const res = await fetch(req.url, { method: httpMethod, headers: req.headers, mode: "cors", cache: "no-store", redirect: "manual", signal: ctrl.signal, ...extra });
      let body = "";
      if (res.type !== "opaqueredirect") { try { body = (await res.text()).slice(0, 300); } catch {} }
      return { status: res.type === "opaqueredirect" ? 302 : res.status, redirect: res.type === "opaqueredirect", body };
    } catch (err) {
      return { status: null, error: err.name === "AbortError" ? "timeout" : "cors" };
    } finally { clearTimeout(t); }
  }
  const refused = r => r && (r.status === 401 || r.status === 403 || r.redirect);
  const answered = r => r && r.status != null && r.status >= 200 && r.status < 300;
  // Reached the application without passing auth: model binding / validation / a server error happened
  const reachedApp = r => r && [400, 415, 422].includes(r.status) || (r && r.status >= 500);
  const say = r => !r ? "not sent" : r.error === "timeout" ? "no answer (timeout)" : r.error ? "blocked by CORS / unreachable" : r.redirect ? "redirect (sign-in)" : `HTTP ${r.status}`;

  // ---------- CORS ----------
  // From this page we can tell: CORS blocks this origin; or allows it with a wildcard (*), which a credentialed request
  // exposes because browsers refuse "*" with credentials; or names this origin specifically.
  async function corsCheck(url) {
    const plain = await send("GET", { url, headers: {} });
    if (plain.error === "timeout") return { restricted: null, text: "CORS: no answer, not checked." };
    if (plain.error) {
      // Reachable at all? (no-cors gets an opaque answer when the server responds)
      try { await fetch(url, { mode: "no-cors", cache: "no-store" }); return { restricted: true, text: "CORS: this origin is not allowed (CORS restricted to other origins)." }; }
      catch { return { restricted: null, text: "CORS: unreachable from here, not checked." }; }
    }
    const withCreds = await send("GET", { url, headers: {} }, { credentials: "include" });
    if (withCreds.error) return { restricted: false, text: "CORS: open to every website (Access-Control-Allow-Origin: *)." };
    return { restricted: true, text: `CORS: allows ${location.origin} by name (restricted to listed origins, unless the API echoes every origin back).` };
  }

  // ---------- IP exposure ----------
  const isPrivate = ip => /^(10\.|127\.|192\.168\.|169\.254\.|172\.(1[6-9]|2\d|3[01])\.|100\.(6[4-9]|[7-9]\d|1[01]\d|12[0-7])\.)/.test(ip) || /^(::1|f[cd]|fe80)/i.test(ip);
  const isIp = h => /^\d{1,3}(\.\d{1,3}){3}$/.test(h) || h.includes(":");
  const dnsCache = new Map();
  async function resolve(host) {
    if (isIp(host)) return host;
    if (dnsCache.has(host)) return dnsCache.get(host);
    const p = (async () => {
      for (const tpl of (CFG.poller?.dns || ["https://dns.google/resolve?name={host}&type={type}"])) {
        try {
          const res = await fetch(tpl.replace("{host}", encodeURIComponent(host)).replace("{type}", "A"), { headers: { accept: "application/dns-json" }, cache: "no-store" });
          const d = await res.json(); const a = (d.Answer || []).find(x => x.type === 1);
          if (a) return a.data;
        } catch {}
      }
      return "";
    })();
    dnsCache.set(host, p);
    return p;
  }
  const OPEN_RANGE = /^(\*|any|all|0\.0\.0\.0(\/0)?|::\/0|internet)$/i;
  async function ipCheck(e, host, url) {
    let hostName = ""; try { hostName = new URL(url).hostname; } catch {}
    const ip = hostName ? await resolve(hostName) : "";
    if (ip && isPrivate(ip)) return { restricted: true, ip, text: `IP: ${hostName} is a private address (${ip}), so it can't be reached from the internet.` };
    const ranges = [val(e.allowedRanges), val(host?.allowedRanges)].find(Boolean) || "";
    const list = ranges.split(/[,;\s]+/).filter(Boolean);
    if (list.length && !list.some(r => OPEN_RANGE.test(r)))
      return { restricted: true, ip, text: `IP: restricted to ${list.slice(0, 4).join(", ")}${list.length > 4 ? "…" : ""} (Allowed Ranges recorded on the ${val(e.allowedRanges) ? "endpoint" : "host"}; not testable from this browser).` };
    return { restricted: false, ip, text: `IP: ${ip ? `public address ${ip}` : "public name"} and no Allowed Ranges recorded on the endpoint or host, so anyone on the internet can reach it.` };
  }

  // ---------- the check ----------
  // e: Apiaudit record, url: the target URL, host: its Apihost, httpMethod: GET/POST/...
  // Resolves { strict, practical, state, cors, ip, findings, stamp }
  async function check(e, url, host, httpMethod = "GET", opts = {}) {
    const f = [], add = (level, text) => f.push({ level, text: "Auth: " + text });
    const c = credsFor(e);
    const method = c.method && c.method !== "auto" ? c.method : inferMethod(e, url);
    let path = ""; try { path = new URL(url.replace(/[{}]/g, "")).pathname; } catch {}
    const isPublic = (AC.publicRoutes || []).some(p => { try { return new RegExp(p, "i").test(path); } catch { return false; } });
    const write = httpMethod !== "GET" && httpMethod !== "HEAD";
    const probeWrites = opts.probeWrites ?? AC.probeWrites;
    let state = "unverified", detail = "";
    const res = {};

    if (write && !probeWrites) {
      detail = `${httpMethod} isn't sent (tick "Also probe writes"); Swagger ${e.hasAuthEnabled ? `declares ${val(e.authType) || "authentication"}` : "declares no authentication"}.`;
      add(e.hasAuthEnabled ? "info" : "warn", `not verified: ${detail}`);
    } else {
      // Writes are only ever sent without credentials or with invalid ones, and with no body
      res.anon = method === "pathToken" ? null : await send(httpMethod, build(method, c, url, "anon"));
      res.bad = await send(httpMethod, build(method === "none" ? "bearer" : method, c, url, "bad"));
      const goodReq = write ? null : build(method, c, url, "good");
      res.good = goodReq ? await send(httpMethod, goodReq) : null;
      const probes = [["no credentials", res.anon], ["invalid " + (method === "basic" ? "username / password" : method === "apikey" ? "API key" : "token"), res.bad]].filter(([, r]) => r);
      const trail = `${probes.map(([n, r]) => `${n} → ${say(r)}`).join("; ")}${res.good ? `; valid ${method === "basic" ? "login" : "token"} → ${say(res.good)}` : ""}`;
      if (probes.some(([, r]) => answered(r))) {
        const open = probes.find(([, r]) => answered(r));
        state = open[0] === "no credentials" ? "open" : "acceptsBad";
        detail = state === "open" ? `answers without any credentials (${trail}).` : `accepts an invalid ${method === "basic" ? "login" : "token"}, so it isn't validated (${trail}).`;
      } else if (probes.some(([, r]) => reachedApp(r))) {
        state = "open"; detail = `reached the application without valid credentials (${trail}): auth isn't checked before the code runs.`;
      } else if (probes.length && probes.every(([, r]) => refused(r))) {
        state = "enforced"; detail = `refuses calls without a valid ${method === "basic" ? "login" : method === "apikey" ? "API key" : "token"} (${trail}).`;
      } else {
        detail = `couldn't confirm (${trail}).` + (probes.some(([, r]) => r.error) ? " CORS must allow this origin and the Authorization header for the probes to be read." : " A 404 / 405 usually means the route needs real parameter values.");
      }
      if (isPublic && (state === "open")) { state = "public"; detail = `public by design (matches config.js authCheck.publicRoutes): ${trail}.`; }
      add({ enforced: "pass", public: "info", open: "fail", acceptsBad: "fail", unverified: "warn" }[state], ({ enforced: "token / login enforced: ", public: "", open: "NOT enforced: ", acceptsBad: "NOT enforced: ", unverified: "not verified: " }[state]) + detail);
      if (state === "enforced" && res.good) {
        if (res.good.status === 401) add("warn", `valid ${method === "basic" ? "username / password" : "token"} was refused (401). Check the account, or that the token is still a live session.`);
        else if (res.good.status === 403) add("info", "valid credentials accepted but not permitted (403): role-based access is working.");
        else if (res.good.error) add("info", `valid-credential call couldn't be read (${say(res.good)}).`);
        else add("pass", `valid credentials let the call through (${say(res.good)}).`);
      } else if (state === "enforced" && !write) add("info", method === "basic" ? "no username / password entered for this endpoint, so a valid login wasn't tried (Credentials…)." : "no valid token available, so a valid call wasn't tried.");
    }

    const strict = state === "enforced" || state === "public";
    let cors = { restricted: null, text: "" }, ip = { restricted: null, text: "" };
    if (!strict) {
      cors = await corsCheck(String(url).replace(/\{[^}]+\}/g, AC.sampleValue));
      ip = await ipCheck(e, host, url);
      add(cors.restricted ? "info" : cors.restricted === false ? "warn" : "info", cors.text.replace(/^CORS: /, "CORS "));
      add(ip.restricted ? "info" : "warn", ip.text.replace(/^IP: /, "IP "));
    }
    const practical = strict || (cors.restricted === true && ip.restricted === true);
    // Kept from the world by CORS + IP: an open route is a warning (strict gap), not a failure
    if (practical && !strict) f.forEach(x => { if (x.level === "fail") x.level = "warn"; });
    const verdict = state === "unverified" && !practical ? null : strict;
    add(strict ? "pass" : practical ? "warn" : verdict === null ? "warn" : "fail",
      `Strict ${strict ? "compliant" : verdict === null ? "not verified" : "NOT compliant"} · Practical ${practical ? "compliant" + (strict ? "" : " (CORS and IP restricted)") : "NOT compliant"}.`);
    const result = `${{ enforced: "Enforced", public: "Public by design", open: "OPEN", acceptsBad: "Accepts invalid credentials", unverified: "Not verified" }[state]} (${method}); ` +
      `strict ${strict ? "yes" : "no"}, practical ${practical ? "yes" : "no"}; ${[cors.text, ip.text].filter(Boolean).join(" ")}`.trim();
    return {
      state, method, strict, practical, cors, ip, findings: f, probes: res,
      stamp: {
        authEnforced: state === "unverified" ? null : strict,
        corsRestricted: cors.restricted, ipRestricted: ip.restricted,
        authCheckResult: result.slice(0, 400), authCheckDate: new Date().toISOString()
      }
    };
  }

  // ---------- badges ----------
  function badges(a) {
    if (!a) return "";
    const s = { enforced: ["success", "Token ✓"], public: ["info", "Public"], open: ["danger", "Open"], acceptsBad: ["danger", "Bad token OK"], unverified: ["secondary", "Auth ?"] }[a.state];
    return `<span class="badge text-bg-${s[0]}" title="${a.method}">${s[1]}</span> ` +
      `<span class="badge ${a.strict ? "text-bg-success" : "text-bg-light border text-danger"}" title="Strict view">S</span>` +
      `<span class="badge ${a.practical ? "text-bg-success" : "text-bg-light border text-danger"} ms-1" title="Practical view">P</span>`;
  }

  // ---------- credentials dialog ----------
  // targets: [{ kind: "host" | "endpoint", id, label, sample (an endpoint for Auto) }]
  function openCredentials(targets, onSaved) {
    const esc = window.Cocky.esc;
    let el = document.getElementById("acModal");
    if (!el) {
      document.body.insertAdjacentHTML("beforeend", `<div class="modal fade" id="acModal" tabindex="-1"><div class="modal-dialog modal-lg modal-dialog-scrollable"><div class="modal-content"><form autocomplete="off">
        <div class="modal-header"><h5 class="modal-title">Credentials for the token check</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
        <div class="modal-body"><p class="small text-muted">Kept in this tab's memory only (never saved, gone when you leave the page). Used for the <b>valid</b> call;
          the no-credential and invalid-credential calls are made either way. <b>Auto</b> picks a token in the path for routes like <code>/api/weather/{token}</code>,
          Basic for WordPress (<code>/wp-json/</code>) or an Auth Type of Basic, and otherwise your signed-in token as a Bearer header.</p><div id="acRows"></div></div>
        <div class="modal-footer"><button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button><button type="submit" class="btn btn-primary">Use these</button></div>
      </form></div></div></div>`);
      el = document.getElementById("acModal");
    }
    const cur = t => (t.kind === "host" ? creds.host.get(t.id) : creds.endpoint.get(t.id)) || { method: "auto" };
    el.querySelector("#acRows").innerHTML = targets.map((t, i) => { const c = cur(t);
      return `<div class="border rounded p-2 mb-2" data-i="${i}">
        <div class="d-flex flex-wrap justify-content-between gap-2 mb-1"><b>${esc(t.label)}</b>${t.kind === "endpoint" ? '<span class="badge text-bg-light border">this endpoint only</span>' : '<span class="badge text-bg-light border">every endpoint on this host</span>'}</div>
        <div class="row g-2 align-items-end">
          <div class="col-md-4"><label class="form-label small mb-0">Method</label><select class="form-select form-select-sm ac-m">${METHODS.map(m => `<option value="${m}" ${c.method === m ? "selected" : ""}>${LABELS[m]}</option>`).join("")}</select></div>
          <div class="col-md-8 ac-f"></div>
        </div>${t.kind === "endpoint" && creds.endpoint.has(t.id) ? '<div class="form-check mt-1"><input class="form-check-input ac-clear" type="checkbox" id="acclr' + i + '"><label class="form-check-label small" for="acclr' + i + '">Clear this override (use the host\'s)</label></div>' : ""}
      </div>`; }).join("");
    const fields = (box, m, c) => {
      const inp = (cls, lbl, v, type = "text", ph = "") => `<div class="col"><label class="form-label small mb-0">${lbl}</label><input class="form-control form-control-sm ${cls}" type="${type}" value="${esc(v || "")}" placeholder="${esc(ph)}" autocomplete="new-password"></div>`;
      box.innerHTML = `<div class="row g-2">${
        m === "basic" ? inp("ac-u", "Username", c.user) + inp("ac-p", "Password / application password", c.pass, "password") :
        m === "apikey" ? inp("ac-h", "Header", c.header || AC.apiKeyHeader) + inp("ac-k", "Key", c.key, "password") :
        m === "bearer" || m === "pathToken" ? inp("ac-t", "Token (blank = your signed-in token)", c.token, "password", signedInToken() ? "signed-in token" : "no signed-in token") :
        m === "auto" ? `<div class="col small text-muted">Signed-in token, or the username / password below for Basic routes.</div>` + inp("ac-u", "Username (Basic)", c.user) + inp("ac-p", "Password", c.pass, "password") :
        '<div class="col small text-muted">Only the no-credential and invalid-credential calls are made.</div>'}</div>`;
    };
    el.querySelectorAll("[data-i]").forEach(row => { const t = targets[+row.dataset.i], sel = row.querySelector(".ac-m");
      fields(row.querySelector(".ac-f"), sel.value, cur(t)); sel.onchange = () => fields(row.querySelector(".ac-f"), sel.value, cur(t)); });
    const bs = bootstrap.Modal.getOrCreateInstance(el);
    el.querySelector("form").onsubmit = ev => {
      ev.preventDefault();
      el.querySelectorAll("[data-i]").forEach(row => {
        const t = targets[+row.dataset.i], map = t.kind === "host" ? creds.host : creds.endpoint;
        if (row.querySelector(".ac-clear")?.checked) { map.delete(t.id); return; }
        const g = c => row.querySelector(c)?.value ?? "";
        const c = { method: row.querySelector(".ac-m").value, token: g(".ac-t").trim(), user: g(".ac-u").trim(), pass: g(".ac-p"), header: g(".ac-h").trim(), key: g(".ac-k") };
        if (t.kind === "endpoint" && c.method === "auto" && !c.user && !c.pass) map.delete(t.id); else map.set(t.id, c);
      });
      bs.hide(); onSaved?.();
    };
    bs.show();
  }

  window.AuthCheck = { CONFIG: AC, check, badges, openCredentials, credsFor, hasOwn, inferMethod, METHODS, LABELS, _test: { build, isPrivate, refused, reachedApp } };
})();
