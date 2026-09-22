// facebook.js
const API_BASE = "https://cockyinvestments-ghehe0a8a7cge7fk.westus3-01.azurewebsites.net";
const FACEBOOK_APP_ID = "488902828130530";
const FACEBOOK_CLIENT_TOKEN = "9da6533f42e1d7c226aaea122ea0a54b";

// Load Facebook SDK
(function loadFacebookSdk() {
  if (window.FB) return;

  window.fbAsyncInit = function () {
    FB.init({
      appId: FACEBOOK_APP_ID,
      clientToken: FACEBOOK_CLIENT_TOKEN,
      cookie: true,
      xfbml: false,
      version: "v19.0"
    });
  };

  const s = document.createElement("script");
  s.src = "https://connect.facebook.net/en_US/sdk.js";
  s.async = true;
  document.head.appendChild(s);
})();

function facebookLogin(onSuccess, onError) {
  if (!window.FB) {
    onError("Facebook SDK not loaded yet.");
    return;
  }

  FB.login(async (resp) => {
    if (!resp.authResponse) {
      onError("Facebook login cancelled.");
      return;
    }

    const fbToken = resp.authResponse.accessToken;

    FB.api("/me", { fields: "email,first_name,last_name,name" }, async (profile) => {
      try {
        const firstname = profile.first_name || "";
        const lastname = profile.last_name || "";
        const fullname = profile.name || `${firstname} ${lastname}`;
        const email = profile.email || `${firstname}.${lastname}@facebook.com`;

        const res = await fetch(`${API_BASE}/api/Auth/loginFacebook`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ email, facebookToken: fbToken, firstname, lastname })
        });

        if (!res.ok) {
          const body = await res.json().catch(() => ({}));
          throw new Error(body.message || "Facebook login failed");
        }

        const data = await res.json();
        onSuccess(data);
      } catch (err) {
        onError(err.message);
      }
    });
  }, { scope: "email,public_profile" });
}
