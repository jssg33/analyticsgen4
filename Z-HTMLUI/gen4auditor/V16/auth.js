// CockyAuditor sign-in guard.
// Load this FIRST, in <head>, on every page. Pages are protected unless the tag has data-public (see below). It runs before the page body is drawn:
// if there's no signed-in session, the page is hidden and the browser goes to login.html.
//
// Sign-in (build 5.2) goes through the API: login.html POSTs { username, plainPassword } to config.js auth.loginPaths
// (/api/Auth/signin, then /api/Auth/login) and stores what comes back. The session lives in localStorage:
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

  function read(key) {
    try { return localStorage.getItem(key); } catch { return null; }
  }
  function write(key, value) {
    try { value == null || value === "" ? localStorage.removeItem(key) : localStorage.setItem(key, String(value)); } catch {}
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

  // Resolves { ok: true } or { ok: false, message }. Stores the session on success.
  async function signIn(username, password) {
    const c = cfg(), t = c.testLogin;
    if (t && t.enabled && username.toLowerCase() === String(t.username).toLowerCase() && password === t.password) {
      store({ ...t.session, token: t.token, testlogin: "1", sessionid: "" });
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
      if (res.status === 400 || res.status === 401) return { ok: false, message: "Username or password is incorrect." };
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
    return { ok: true };
  }

  // to: where to go afterwards (the login page by default; the public Welcome page just reloads itself)
  function signOut(to = "login.html") {
    const s = current(), path = cfg().logoutPath;
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

  window.CockyAuth = { isSignedIn, signIn, signOut, current, safeNext, tokenPayload, tokenExpired, LOGGED_OUT_ID };

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
