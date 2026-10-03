// CockyAuditor sign-in guard.
// Load this FIRST, in <head>, on every page. Pages are protected unless the tag has data-public (see below). It runs before the page body is drawn:
// if there's no signed-in session, the page is hidden and the browser goes to login.html.
//
// Sign-in (build 5.2) goes through the API: login.html POSTs { username, plainPassword } to config.js auth.loginPaths
// (/api/Auth/signin, then /api/Auth/login) and stores what comes back. The session lives in localStorage, each value
// stored as caesar96 JSON through CockyCiphers.store (js/cockyciphers.js):
//   userid     the user's id ("901" when signed out)
//   role       the user's role from the API ("auditor" for the test login)
//   username, email, fullname
//   token      the JWT from the API; its "exp" is checked on every page (not for the test login)
//   sessionid  the Usersession id, when the API returns one
//   testlogin  "1" when signed in with the audit / audit test login (config.js auth.testLogin)
//
// This is still a client-side check: it decides which pages to draw, it doesn't protect the API. The API has to check
// the token itself on every route that needs one.
(function () {
  const LOGGED_OUT_ID = "901";
  const KEYS = ["role", "username", "email", "fullname", "token", "sessionid", "testlogin"];
  const cfg = () => (window.COCKY_CONFIG && window.COCKY_CONFIG.auth) || {};

  // localStorage goes only through CockyCiphers.store (js/cockyciphers.js, loaded before this file on every page):
  // every value is stored as caesar96 JSON. Without the library nothing is read or written, so the page asks for sign-in.
  const STORE = window.CockyCiphers && window.CockyCiphers.store;
  if (!STORE) console.error("CockyAuditor: js/cockyciphers.js must be loaded before auth.js");
  else STORE.migrateAll();
  function read(key) {
    return STORE ? STORE.get(key) : null;
  }
  function write(key, value) {
    if (!STORE) return;
    value == null || value === "" ? STORE.remove(key) : STORE.set(key, String(value));
  }

  function current() {
    return { userid: read("userid"), role: read("role"), username: read("username"), email: read("email"),
             fullname: read("fullname"), token: read("token"), sessionid: read("sessionid"), testLogin: read("testlogin") === "1" };
  }

  // JWT payload, decoded without verifying it (only the API can verify). null if it isn't a JWT.
  function tokenPayload(t) {
    const part = String(t || "").split(".")[1];
    if (!part) return null;
    try {
      const b64 = part.replace(/-/g, "+").replace(/_/g, "/").padEnd(Math.ceil(part.length / 4) * 4, "=");
      return JSON.parse(decodeURIComponent(atob(b64).split("").map(c => "%" + c.charCodeAt(0).toString(16).padStart(2, "0")).join("")));
    } catch { return null; }
  }
  function tokenExpired(t) {
    const p = tokenPayload(t);
    return !!(p && typeof p.exp === "number" && p.exp * 1000 <= Date.now());
  }

  // Signed in = a user id that isn't the signed-out id, a token, and (except for the test login) a token that hasn't expired
  function isSignedIn() {
    const s = current();
    if (!s.userid || s.userid === LOGGED_OUT_ID || !s.token) return false;
    return s.testLogin || !tokenExpired(s.token);
  }

  function roleAllowed(role) {
    const allowed = (cfg().allowedRoles || []).map(r => String(r).toLowerCase());
    return !allowed.length || allowed.includes(String(role || "").toLowerCase());
  }

  function store(session) {
    write("userid", session.userid);
    KEYS.forEach(k => write(k, session[k]));
  }

  function apiUrl(path) {
    if (/^https?:\/\//i.test(path)) return path;
    const base = (window.COCKY_CONFIG?.apiBaseUrl || "").replace(/\/$/, "");
    return base + path;
  }

  // ---------- access logging (build 6.0) ----------
  // Every sign-in to CockyAuditor is written to the API's own logs (config.js accessLog): Userlog (the user signed in),
  // Syslog (auditor access note) and Superuserlog (access to the tool). Failed sign-ins go to Syslog, sign-outs to Userlog,
  // and pages can add their own events (Users -> Profile writes a Syslog line). Field names come from the body templates in
  // config.js; "{placeholders}" are filled in. Fields the API doesn't have are ignored by ASP.NET; if it answers 400 naming
  // fields, numbers / text are swapped for those fields (or a required blank one is filled) and it's sent once more.
  const logCfg = () => (window.COCKY_CONFIG && window.COCKY_CONFIG.accessLog) || {};
  function fillTemplate(tpl, vars) {
    const out = {};
    Object.entries(tpl || {}).forEach(([k, v]) => {
      if (typeof v !== "string") { out[k] = v; return; }
      const whole = /^\{(\w+)\}$/.exec(v);
      if (whole) { const x = vars[whole[1]]; out[k] = x === undefined || x === "" ? null : x; return; }
      out[k] = v.replace(/\{(\w+)\}/g, (_, n) => vars[n] == null ? "" : String(vars[n]));
    });
    return out;
  }
  function logVars(event, note, who) {
    const s = who || current(), CFG = window.COCKY_CONFIG || {};
    const uid = s.userid && s.userid !== LOGGED_OUT_ID && /^\d+$/.test(String(s.userid)) ? Number(s.userid) : null;
    return {
      event, note, now: new Date().toISOString(), userid: uid, useridtext: uid == null ? "" : String(uid),
      username: s.username || "", fullname: s.fullname || "", email: s.email || "", role: s.role || "",
      sessionid: s.sessionid && /^\d+$/.test(String(s.sessionid)) ? Number(s.sessionid) : null,
      testlogin: s.testLogin || s.testlogin === "1" ? 1 : 0, app: CFG.appName || "CockyAuditor", build: CFG.version || "",
      origin: location.origin && location.origin !== "null" ? location.origin : location.protocol + "//" + location.host + location.pathname,
      page: location.pathname.split("/").pop() || "index.html", agent: navigator.userAgent.slice(0, 200)
    };
  }
  async function postLog(name, body, { keepalive = false } = {}) {
    const def = (logCfg().logs || {})[name];
    if (!def || !def.path) return { name, ok: false, message: "no route in config.js accessLog.logs" };
    const target = apiUrl(def.path);
    let current_ = { ...body };
    for (let round = 0; round < 3; round++) {
      const ctrl = new AbortController(), timer = setTimeout(() => ctrl.abort(), logCfg().timeoutMs || 5000);
      let res, text = "";
      try {
        res = await fetch(target, { method: "POST", keepalive, signal: ctrl.signal,
          headers: { "Content-Type": "application/json", Accept: "application/json" }, body: JSON.stringify(current_) });
        text = await res.text();
      } catch (e) { return { name, ok: false, message: e.name === "AbortError" ? "timed out" : "couldn't reach " + target + " (CORS or API down)" }; }
      finally { clearTimeout(timer); }
      if (res.ok) return { name, ok: true };
      let data = null; try { data = JSON.parse(text); } catch {}
      const errs = data && data.errors ? Object.keys(data.errors) : [];
      if (res.status !== 400 || !errs.length) return { name, ok: false, message: `${res.status} ${res.statusText}${data && data.title ? " - " + data.title : ""}` };
      let changed = false;
      errs.forEach(raw => {
        const k = raw.replace(/^\$\./, "").split(/[.\[]/)[0];
        const key = Object.keys(current_).find(x => x.toLowerCase() === k.toLowerCase()) || (k ? k[0].toLowerCase() + k.slice(1) : "");
        if (!key || /^(body|request|item|input)$/i.test(key)) return;
        const v = current_[key];
        if (typeof v === "number") { current_[key] = String(v); changed = true; }
        else if (typeof v === "string" && /^-?\d+$/.test(v)) { current_[key] = Number(v); changed = true; }
        else if (v == null || v === "") { current_[key] = /id$|count|level|priority/i.test(key) ? 0 : "N/A"; changed = true; }
      });
      if (!changed) return { name, ok: false, message: `400 - ${errs.join(", ")}` };
    }
    return { name, ok: false, message: "the API kept rejecting the record" };
  }
  // Writes `event` to the logs listed for it in accessLog.events (or the given names). Never throws.
  // Resolves [{ name, ok, message }]. Failures are kept in sessionStorage and shown once by the next page.
  async function logEvent(event, note, opts = {}) {
    const c = logCfg();
    if (c.enabled === false) return [];
    const names = opts.logs || (c.events || {})[event] || [];
    const vars = logVars(event, note, opts.who);
    const results = await Promise.all(names.map(n => postLog(n, fillTemplate((c.logs[n] || {}).body, vars), opts).catch(e => ({ name: n, ok: false, message: e.message }))));
    const bad = results.filter(r => !r.ok);
    if (bad.length && !opts.keepalive) {
      try { sessionStorage.setItem("cocky.accesslog.warn", `${event}: ${bad.map(r => `${r.name} (${r.message})`).join("; ")}`); } catch {}
      console.warn("CockyAuditor access log:", event, bad);
    }
    return results;
  }
  const describe = s => `${s.username || s.email || "user #" + s.userid} (user #${s.userid}, role ${s.role || "none"}${s.sessionid ? ", session #" + s.sessionid : ""}${s.testLogin || s.testlogin === "1" ? ", test login" : ""})`;
  async function logSignIn() {
    const s = current(), CFG = window.COCKY_CONFIG || {}, from = location.origin && location.origin !== "null" ? location.origin : "a local copy";
    // The three sign-in logs, each with its own note
    const notes = {
      userlog: `Signed in to ${CFG.appName || "CockyAuditor"} ${CFG.version ? "build " + CFG.version : ""}`.trim(),
      syslog: `Auditor access: ${describe(s)} signed in to ${CFG.appName || "CockyAuditor"} from ${from}`,
      superuserlog: `${CFG.appName || "CockyAuditor"} tool access granted to ${describe(s)}`
    };
    const names = (logCfg().events || {}).signin || [];
    const all = await Promise.all(names.map(n => logEvent("signin", notes[n] || notes.syslog, { logs: [n] })));
    return all.flat();
  }

  // ---------- test-login session (build 6.4) ----------
  // audit / audit does NOT call the sign-in API (it works offline), but it still opens a REAL Usersession:
  // it generates its own 128-bit token and POSTs it to /api/Usersession/{userId} for the auditor's real user
  // (config auth.testLogin.session.userid), then stores the session id the API returns. Best effort: if the API
  // can't be reached, the token is still generated and the login works offline with no session id.
  const reqTimeout = () => window.COCKY_CONFIG?.requestTimeoutMs || 30000;
  // A fresh 128-bit (16-byte) token, hex-encoded. Uses Web Crypto; falls back to Math.random only if it's unavailable.
  function gen128() {
    const g = window.crypto || window.msCrypto;
    if (g && g.getRandomValues) { const a = new Uint8Array(16); g.getRandomValues(a); return Array.from(a, b => b.toString(16).padStart(2, "0")).join(""); }
    let s = ""; for (let i = 0; i < 32; i++) s += Math.floor(Math.random() * 16).toString(16); return s;
  }
  async function apiPost(path, body) {
    const ctrl = new AbortController(), timer = setTimeout(() => ctrl.abort(), reqTimeout());
    try {
      const res = await fetch(apiUrl(path), { method: "POST", signal: ctrl.signal,
        headers: { "Content-Type": "application/json", Accept: "application/json" }, body: JSON.stringify(body || {}) });
      const text = await res.text(); let data = null; try { data = JSON.parse(text); } catch { data = text; }
      return { ok: res.ok, status: res.status, data };
    } finally { clearTimeout(timer); }
  }
  const pickId = (d, keys) => { if (d && typeof d === "object") for (const k of keys) { const hit = Object.keys(d).find(x => x.toLowerCase() === k.toLowerCase()); if (hit && d[hit] != null) return d[hit]; } return null; };
  // Generates a 128-bit token and opens a Usersession for the auditor's real user. Returns { token, sessionid }.
  async function provisionTestLogin(t) {
    const EP = (window.COCKY_CONFIG && window.COCKY_CONFIG.endpoints) || {};
    const sess = t.session || {};
    const token = gen128();
    const out = { token, sessionid: null };
    const uid = sess.userid;
    if (t.provision !== false && uid != null && EP.usersession) {
      // POST /api/Usersession/{userId} with our generated token so the session row carries it.
      const body = { token, userid: /^\d+$/.test(String(uid)) ? Number(uid) : uid, sessiondescription: "CockyAuditor test login", acknowledged: true };
      try { const r = await apiPost(EP.usersession.replace(/\/$/, "") + "/" + encodeURIComponent(uid), body); if (r.ok) { const sid = pickId(r.data, ["sessionId", "id", "sessionid"]); if (sid != null) out.sessionid = String(sid); } } catch {}
    }
    return out;
  }

  // Resolves { ok: true } or { ok: false, message }. Stores the session on success.
  async function signIn(username, password) {
    const c = cfg(), t = c.testLogin;
    if (t && t.enabled && username.toLowerCase() === String(t.username).toLowerCase() && password === t.password) {
      const session = { ...t.session, token: t.token, testlogin: "1", sessionid: "" };
      try {
        const prov = await provisionTestLogin(t);
        session.token = prov.token || t.token;           // use the freshly generated 128-bit token
        if (prov.sessionid != null) session.sessionid = prov.sessionid;
      } catch (e) { console.warn("CockyAuditor test-login session:", e.message); }
      store(session);
      await logSignIn();
      return { ok: true, test: true };
    }
    const paths = c.loginPaths?.length ? c.loginPaths : ["/api/Auth/login"];
    let res, data, tried = [];
    for (const path of paths) {
      const target = apiUrl(path);
      tried.push(target);
      const ctrl = new AbortController();
      const timer = setTimeout(() => ctrl.abort(), window.COCKY_CONFIG?.requestTimeoutMs || 30000);
      try {
        res = await fetch(target, { method: "POST", signal: ctrl.signal,
          headers: { "Content-Type": "application/json", Accept: "application/json" },
          body: JSON.stringify({ username, plainPassword: password }) });
      } catch (e) {
        return { ok: false, message: e.name === "AbortError" ? "The sign-in service didn't answer in time. Try again."
          : `Couldn't reach the sign-in service. The API must allow this site (${location.origin}) in CORS.` };
      } finally { clearTimeout(timer); }
      if (res.status === 404 || res.status === 405) continue;     // route not deployed under this name: try the next one
      break;
    }
    if (!res || res.status === 404 || res.status === 405)
      return { ok: false, message: `No sign-in route answered (${tried.join(", ")}).` };
    const text = await res.text();
    try { data = JSON.parse(text); } catch { data = text; }
    if (!res.ok) {
      // The API says "User not found." / "Password mismatch."; don't tell the visitor which one it was
      if (res.status === 400 || res.status === 401) {
        await logEvent("signinFailed", `Failed ${window.COCKY_CONFIG?.appName || "CockyAuditor"} sign-in for "${String(username).slice(0, 100)}" from ${location.origin}`,
          { who: { userid: null, username: String(username).slice(0, 100) } });
        return { ok: false, message: "Username or password is incorrect." };
      }
      return { ok: false, message: `Sign-in failed (${res.status}). Try again later.` };
    }
    const get = (...keys) => { for (const k of keys) { const hit = Object.keys(data || {}).find(x => x.toLowerCase() === k.toLowerCase()); if (hit && data[hit] != null) return data[hit]; } };
    const token = get("token");
    const userid = get("userId", "id");
    if (!token || userid == null) return { ok: false, message: "The sign-in service answered without a token. Check the API." };
    if (tokenExpired(token)) return { ok: false, message: "The sign-in service returned a token that has already expired. Check the server clock." };
    const role = get("userRole", "role") || tokenPayload(token)?.role || "";
    if (!roleAllowed(role)) return { ok: false, message: `Your account (role "${role || "none"}") isn't allowed to use CockyAuditor.` };
    store({
      userid: String(userid), role,
      username: get("userUsername", "username") || "",
      email: get("userEmail", "email") || "",
      fullname: get("userFullName", "userFullname", "fullname") || [get("userFirstname"), get("userLastname")].filter(Boolean).join(" "),
      token, sessionid: get("sessionId") ?? "", testlogin: ""
    });
    await logSignIn();
    return { ok: true };
  }

  // to: where to go afterwards (the login page by default; the public Welcome page just reloads itself)
  function signOut(to = "login.html") {
    const s = current(), path = cfg().logoutPath;
    // Sign-out goes to Userlog (keepalive, so it's sent even though the page is leaving)
    if (isSignedIn()) logEvent("signout", `Signed out of ${window.COCKY_CONFIG?.appName || "CockyAuditor"}`, { keepalive: true, who: s });
    // End the Usersession on the API (best effort, not for the test login). id=-1 so the API's
    // "Token == token || Userid == id" lookup only matches this token.
    if (path && s.token && !s.testLogin) {
      try {
        fetch(apiUrl(path.replace(/\/$/, "")) + "/" + encodeURIComponent(s.token) + "?id=-1",
          { method: "PUT", keepalive: true, headers: { "Content-Type": "application/json" }, body: "{}" }).catch(() => {});
      } catch {}
    }
    write("userid", LOGGED_OUT_ID);
    KEYS.forEach(k => write(k, null));
    location.replace(to);
  }

  // Only allow going back to one of our own pages after login (no ?next=https://elsewhere)
  function safeNext(next) {
    return /^[a-z0-9_-]+\.html([?#].*)?$/i.test(next || "") && !/^login\.html/i.test(next) ? next : "index.html";
  }

  function here() {
    const file = location.pathname.split("/").pop() || "index.html";
    return file + location.search + location.hash;
  }

  function goToLogin(reason) {
    document.documentElement.style.display = "none";
    location.replace("login.html?next=" + encodeURIComponent(here()) + (reason ? "&reason=" + reason : ""));
  }
  const expiredNow = () => { const s = current(); return !!s.token && !s.testLogin && tokenExpired(s.token); };

  window.CockyAuth = { isSignedIn, signIn, signOut, current, safeNext, tokenPayload, tokenExpired, logEvent, LOGGED_OUT_ID };

  const isLoginPage = /(^|\/)login\.html$/i.test(location.pathname);
  // A page can opt out of the guard with <script src="auth.js" data-public></script> (the Welcome page does);
  // it can still check CockyAuth.isSignedIn() to show more when someone is signed in.
  const isPublic = !!document.currentScript?.hasAttribute("data-public");
  if (!isLoginPage && !isPublic) {
    if (!isSignedIn()) goToLogin(expiredNow() ? "expired" : "");
    // Back button after signing out can show a cached page; check again when it's shown
    window.addEventListener("pageshow", () => { if (!isSignedIn()) goToLogin(expiredNow() ? "expired" : ""); });
    // Signing out in another tab signs this tab out too
    window.addEventListener("storage", e => {
      if (["userid", "token", null].includes(e.key) && !isSignedIn()) goToLogin();
    });
    // The token can run out while a page is open: check once a minute
    setInterval(() => { if (!isSignedIn()) goToLogin("expired"); }, 60000);
  }
})();
