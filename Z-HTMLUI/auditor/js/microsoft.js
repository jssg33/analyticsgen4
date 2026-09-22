// microsoft.js
const API_BASE = "https://cockyinvestments-ghehe0a8a7cge7fk.westus3-01.azurewebsites.net";
const MICROSOFT_CID = "2a78bf27-2367-4f19-967e-e40466a11592";
const MICROSOFT_TENANT = "57836496-6e32-4a51-bf60-8e4c21bbf622";

// Load MSAL SDK
(function loadMsalSdk() {
  if (window.msal) return;

  const s = document.createElement("script");
  s.src = "https://alcdn.msauth.net/browser/2.38.3/js/msal-browser.min.js";
  s.async = true;
  document.head.appendChild(s);
})();

function microsoftLogin(onSuccess, onError) {
  if (!window.msal) {
    onError("Microsoft SDK not loaded yet.");
    return;
  }

  try {
    const pca = new window.msal.PublicClientApplication({
      auth: {
        clientId: MICROSOFT_CID,
        authority: `https://glocationinfo.ciamlogin.com/${MICROSOFT_TENANT}`
      },
      cache: { cacheLocation: "sessionStorage" }
    });

    pca.loginPopup({ scopes: ["openid", "profile", "email"] })
      .then(async (result) => {
        const idToken = result.idToken;
        const account = result.account;

        const firstname = account.name?.split(" ")[0] || "";
        const lastname = account.name?.split(" ").slice(1).join(" ") || "";
        const rawEmail = account.username || "";
        const email = rawEmail || `${firstname}.${lastname}@hotmail.com`.toLowerCase();

        const res = await fetch(`${API_BASE}/api/Auth/loginMicrosoft`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            email,
            microsoftToken: idToken,
            firstname,
            lastname
          })
        });

        if (!res.ok) {
          const body = await res.json().catch(() => ({}));
          throw new Error(body.message || "Microsoft login failed");
        }

        const data = await res.json();
        onSuccess(data);
      })
      .catch(err => onError(err.message));

  } catch (err) {
    onError(err.message);
  }
}
