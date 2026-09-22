/* ---------------------------------------------------------
   GLOBAL CONFIG
--------------------------------------------------------- */
const API_BASE = "https://cockyinvestments-ghehe0a8a7cge7fk.westus3-01.azurewebsites.net";

/* ---------------------------------------------------------
   FACEBOOK LOGIN (placeholder)
--------------------------------------------------------- */
function facebookLoginPopup() {
    alert("FBLOGIN");
}

/* ---------------------------------------------------------
   GOOGLE LOGIN (placeholder)
--------------------------------------------------------- */
function googleLoginPopup() {
    alert("GLOGIN");
}

/* ---------------------------------------------------------
   MICROSOFT LOGIN (MSAL)
--------------------------------------------------------- */
const MICROSOFT_CID    = "2a78bf27-2367-4f19-967e-e40466a11592";
const MICROSOFT_TENANT = "57836496-6e32-4a51-bf60-8e4c21bbf622";

const msalConfig = {
    auth: {
        clientId: MICROSOFT_CID,
        authority: `https://${MICROSOFT_TENANT}.ciamlogin.com`,   // FIXED
        redirectUri: window.location.origin
    },
    cache: {
        cacheLocation: "sessionStorage"
    }
};

const msalInstance = new msal.PublicClientApplication(msalConfig);

/* ---------------------------------------------------------
   MICROSOFT LOGIN POPUP HANDLER
--------------------------------------------------------- */
async function microsoftLoginPopup() {
    try {
        const response = await msalInstance.loginPopup({
            scopes: ["openid", "profile", "email"]
        });

        const idToken = response.idToken;
        const account = response.account;

        const firstname = account.name?.split(" ")[0] || "";
        const lastname  = account.name?.split(" ").slice(1).join(" ") || "";
        const rawEmail  = account.username || "";
        const email     = rawEmail || `${firstname}.${lastname}@hotmail.com`.toLowerCase();

        // Send to backend
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

        document.getElementById("status").innerHTML =
            "Logged in as: " + email;

        console.log("Microsoft login success:", data);
        return data;

    } catch (err) {
        console.error(err);
        document.getElementById("status").innerHTML =
            "Login failed: " + err.message;
    }
}
