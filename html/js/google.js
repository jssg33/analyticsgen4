// google.js
const API_BASE = "https://cockyinvestments-ghehe0a8a7cge7fk.westus3-01.azurewebsites.net";
const GOOGLE_CID = "108407518954-docr8h68aa11b0jjkts62kq4fh9hvq10.apps.googleusercontent.com";

// Load Google Identity Services SDK
(function loadGoogleSdk() {
  const s = document.createElement("script");
  s.src = "https://accounts.google.com/gsi/client";
  s.async = true;
  s.defer = true;
  document.head.appendChild(s);
})();

function decodeJwt(token) {
  try {
    return JSON.parse(atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
  } catch {
    return {};
  }
}

function googleLogin(onSuccess, onError) {
  if (!window.google || !window.google.accounts) {
    onError("Google SDK not loaded yet.");
    return;
  }

  // Initialize popup mode
  google.accounts.id.initialize({
    client_id: GOOGLE_CID,
    callback: async (resp) => {
      try {
        const payload = decodeJwt(resp.credential);
        const email = payload.email;
        const googleToken = resp.credential;

        const res = await fetch(`${API_BASE}/api/Auth/loginGoogle`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ username: email, googleToken })
        });

        if (!res.ok) {
          const body = await res.json().catch(() => ({}));
          throw new Error(body.message || "Google login failed");
        }

        const data = await res.json();
        onSuccess(data);
      } catch (err) {
        onError(err.message);
      }
    },
    ux_mode: "popup"
  });

  // MUST be triggered by user click
  google.accounts.id.prompt();
}
