// CockyAuditor sign-in guard.
// Load this FIRST, in <head>, on every protected page. It runs before the page body is drawn:
// if the stored session isn't a signed-in auditor, the page is hidden and the browser goes to login.html.
//
// Session lives in localStorage under these keys:
//   userid   "10000" when signed in, "901" when signed out
//   role     must be "auditor"
//   username "auditor"
//   email    "auditor@capitoltechnology.net"
//
// NOTE: this is a client-side, hard-coded test login (audit / audit). Anyone can read it in the page
// source or set these localStorage keys by hand, so it keeps casual visitors out but is not real security.
// Replace it with the API's real login before this goes anywhere public.
(function () {
  const LOGGED_OUT_ID = "901";
  const REQUIRED_ROLE = "auditor";

  const TEST_USER = {
    login: "audit", password: "audit",
    session: { userid: "10000", role: "auditor", username: "auditor", email: "auditor@capitoltechnology.net" }
  };

  function read(key) {
    try { return localStorage.getItem(key); } catch { return null; }
  }
  function write(key, value) {
    try { localStorage.setItem(key, value); } catch {}
  }

  function current() {
    return { userid: read("userid"), role: read("role"), username: read("username"), email: read("email") };
  }

  // Signed in = role is auditor AND userid is set AND userid isn't the signed-out id (901)
  function isSignedIn() {
    const s = current();
    return s.role === REQUIRED_ROLE && !!s.userid && s.userid !== LOGGED_OUT_ID;
  }

  // Returns true and stores the session if the credentials match the test user
  function signIn(login, password) {
    if (login !== TEST_USER.login || password !== TEST_USER.password) return false;
    Object.entries(TEST_USER.session).forEach(([k, v]) => write(k, v));
    return true;
  }

  function signOut() {
    write("userid", LOGGED_OUT_ID);
    ["role", "username", "email"].forEach(k => { try { localStorage.removeItem(k); } catch {} });
    location.replace("login.html");
  }

  // Only allow going back to one of our own pages after login (no ?next=https://elsewhere)
  function safeNext(next) {
    return /^[a-z0-9_-]+\.html([?#].*)?$/i.test(next || "") && !/^login\.html/i.test(next) ? next : "index.html";
  }

  function here() {
    const file = location.pathname.split("/").pop() || "index.html";
    return file + location.search + location.hash;
  }

  function goToLogin() {
    document.documentElement.style.display = "none";
    location.replace("login.html?next=" + encodeURIComponent(here()));
  }

  window.CockyAuth = { isSignedIn, signIn, signOut, current, safeNext, LOGGED_OUT_ID };

  const isLoginPage = /(^|\/)login\.html$/i.test(location.pathname);
  if (!isLoginPage) {
    if (!isSignedIn()) goToLogin();
    // Back button after signing out can show a cached page; check again when it's shown
    window.addEventListener("pageshow", () => { if (!isSignedIn()) goToLogin(); });
    // Signing out in another tab signs this tab out too
    window.addEventListener("storage", e => {
      if (["userid", "role", null].includes(e.key) && !isSignedIn()) goToLogin();
    });
  }
})();
