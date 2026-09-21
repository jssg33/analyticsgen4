  	function setuser() {
    alert("setting user for trial usage");
    localStorage.setItem("uid", "1");
    localStorage.setItem("fullname", "Analytics Guest");
    localStorage.setItem("email", "parkguest@547bikes.info");
    localStorage.setItem("role", "registered");
    localStorage.setItem("status", "loggedin");
    localStorage.setItem("firstname", "Park");
    localStorage.setItem("lastname", "Guest");
}

function checkpll() {
    const someuid = localStorage.getItem('uid');
    if (someuid === '901') {
        window.location.href = 'loginerror.html';
    } else if (!someuid) {
        localStorage.setItem("uid", "901");
        localStorage.setItem("fullname", "Analytics Guest");
    }
}

/* ---------------------------------------------------------
   SAFE PROFILE PICTURE DISPLAY
--------------------------------------------------------- */
function displayProfilePicture() {
    const imageElement = document.getElementById("profilephototarget");
    if (!imageElement) return;  // prevent crash

    let newSrc = './images/blackcircle.png';
    const tempSrc = localStorage.getItem("activepictureurl");

    if (tempSrc) newSrc = tempSrc;

    imageElement.src = newSrc;
}

/* ---------------------------------------------------------
   SAFE USER STATUS CHECKER
--------------------------------------------------------- */
function checkUserStatus() {
    const userId = localStorage.getItem("uid");
    const fullName = localStorage.getItem("fullname") || "Guest";
    const userRole = localStorage.getItem("role") || "guest";

    const loginBtn  = document.getElementById("loginBtn");
    const logoutBtn = document.getElementById("logoutBtn");
    const greeting  = document.getElementById("userGreeting");
    const banner    = document.getElementById("H3UserBanner");
    const profile   = document.getElementById("profilephototarget");

    // SAFE: update only if element exists
    if (greeting) greeting.innerHTML = userId && userId !== "901" ? `Welcome, ${fullName}` : "Welcome, Guest";
    if (banner)   banner.innerText   = fullName;

    if (loginBtn)  loginBtn.style.display  = (userId && userId !== "901") ? "none" : "inline-block";
    if (logoutBtn) logoutBtn.style.display = (userId && userId !== "901") ? "inline-block" : "none";

    if (profile && (!userId || userId === "901")) {
        profile.src = "./images/blackcircle.png";
    }

    // ADMIN AUTO-REDIRECT (only if element exists)
    if (userRole === "admin") {
        window.open("adminhome.html", "_self");
    }
}

/* ---------------------------------------------------------
   LOGOUT
--------------------------------------------------------- */
function logoutUser(event) {
    event.preventDefault();
    console.log("Logging out user...");

    localStorage.clear();
    localStorage.setItem("uid", "901");
    localStorage.setItem("fullname", "Guest");
    localStorage.setItem("role", "guest");

    clearUserSession();
    window.location.replace("login.html");
}

/* ---------------------------------------------------------
   CLOSE SESSION (SAFE)
--------------------------------------------------------- */
function clearUserSession() {
    let sometoken = localStorage.getItem('token');
    if (!sometoken) return;

    let logoutApiUrl = "https://cockyapiv3-bugudue8akcsbacz.westus3-01.azurewebsites.net/api/Users/logout/" + sometoken;

    $.post(logoutApiUrl)
        .done(() => alert("SessionClosed."))
        .fail(xhr => console.error("Error closing session:", xhr.responseText));
}

/* ---------------------------------------------------------
   LOGIN REDIRECT
--------------------------------------------------------- */
function loginUser(event) {
    event.preventDefault();

    localStorage.clear();
    localStorage.setItem("uid", "901");
    localStorage.setItem("fullname", "Guest");
    localStorage.setItem("role", "guest");
    localStorage.setItem("isemployee", "0");

    window.open("login.html", "_self");
}

/* ---------------------------------------------------------
   SAFE PROFILE FETCH
--------------------------------------------------------- */
function fetchProfilePicture() {
    const uid = localStorage.getItem('uid');
    if (!uid) return;

    $.get("https://gemroot-d3h6hybke9c4f0fr.centralus-01.azurewebsites.net/api/Userprofile")
        .done(profiles => {
            const profile = profiles.find(p => p.userid == uid);
            if (profile && profile.activepictureurl) {
                const img = document.getElementById("userProfilePic");
                if (img) img.src = profile.activepictureurl;
            }
        })
        .fail(xhr => console.error("Error fetching profiles:", xhr.responseText));
}

/* ---------------------------------------------------------
   SAFE USER FETCH
--------------------------------------------------------- */
function fetchUser() {
    const uid = localStorage.getItem('uid');
    if (!uid) return;

    $.get(usersApiUrl)
        .done(users => {
            const user = users.find(u => u.id == uid);
            if (!user) return;

            const banner = document.getElementById("H3UserBanner");
            if (banner) banner.textContent = user.fullname;
        })
        .fail(xhr => console.error("Error fetching user:", xhr.responseText));
}

/* ---------------------------------------------------------
   SAFE ROLE CHECK
--------------------------------------------------------- */
function checkUserRole() {
    const userRole = localStorage.getItem("role");
    const manager = document.getElementById("manager");

    if (manager && userRole !== "superuser") {
        manager.style.display = "none";
    }
}

/* ---------------------------------------------------------
   SAFE LOCATION WRITER
--------------------------------------------------------- */
function writelocation() {
    const locElement = document.getElementById("locationbar");
    const ipElement  = document.getElementById("ipbar");

    if (!locElement || !ipElement) return;

    const somefulllocation = localStorage.getItem("userlocation");
    const someipaddress    = localStorage.getItem("ipaddress");

    if (somefulllocation) {
        locElement.innerHTML = `Your Location is: ${somefulllocation}`;
        ipElement.innerHTML  = `Your IP is: ${someipaddress}`;
    } else {
        locElement.innerHTML = "Location data not available.";
        ipElement.innerHTML  = "Your IP address is not available.";
    }
}

/* ---------------------------------------------------------
   SAFE SESSION CLOSE
--------------------------------------------------------- */
function closeUserSession() {
    const uid = localStorage.getItem('uid');
    if (!uid) return;

    const payload = { sessionend: new Date().toISOString() };

    fetch(`https://yellow-cliff-04c6b2d10.7.azurestaticapps.net/api/Usersession/user/${uid}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    })
        .then(r => r.json())
        .then(data => console.log('Session closed successfully:', data))
        .catch(err => console.error('Error closing session:', err));
}

/* SETTING A QUICK QUEST LOGIN */
function guestLogin() {
    // Hard‑coded guest account
    localStorage.setItem("uid", "801");
    localStorage.setItem("fullname", "Guest User");
    localStorage.setItem("firstname", "Guest");
    localStorage.setItem("lastname", "User");
    localStorage.setItem("username", "guest@cockyinvestments.com");
    localStorage.setItem("email", "guest@cockyinvestments.com");
    localStorage.setItem("role", "guest");
    localStorage.setItem("status", "loggedin");
    localStorage.setItem("isemployee", "0");

    // Default guest profile picture
    localStorage.setItem("activepictureurl", "./images/defaultcircle.png");
    localStorage.setItem("profileurl", "./images/defaultcircle.png");
    localStorage.setItem("activeurl", "./images/blackcircle.png");

    // Optional: set a fake token so your session logger doesn't break
    localStorage.setItem("token", "guest-token-801");

    console.log("Guest login activated.");

    // Redirect to home page
    window.location.href = "index.html";
}

