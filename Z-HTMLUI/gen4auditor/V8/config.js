// CockyAuditor configuration.
// This used to be config.json loaded with fetch(), which fails when the pages
// are opened straight from disk (file://). Keeping it as a script works both
// from disk and from a web server.
window.COCKY_CONFIG = {
  appName: "CockyAuditor",
  // Current build, shown on the home page and in the sidebar. Add each new build to the top of "builds".
  version: "4.7",
  // Copyright line shown on every page
  copyright: "\u00A9 2026-2027 CockyFinancial Services, Inc.",
  builds: [
    { build: "4.8", title: "Back-end interfaces", notes: "Interfaces page audits the services and interfaces registered behind an API's back-end (from the Service Controller's SystemConsole enumeration), stored in ApiInterfacesAudit. Import from Console pastes the SystemConsole JSON and saves one row per interface method (interface, method, implementation, lifetime, registered), tied to an endpoint. Filter by endpoint; Interfaces tile on the Dashboard. Discovery only — the auditor never invokes the services it records." },
    { build: "4.7", title: "Public welcome", notes: "Welcome page opens without signing in, with a top bar: Help (trouble tickets, /api/Userhelp), Notices (/api/usernotices), Profile (/api/userprofile) and Sign in / Sign out. Help, Notices and Profile need sign-in (signed out: a login error, then the sign-in form). Links from Welcome to the inside pages check the login first." },
    { build: "4.6", title: "Enterprise(9) page", notes: "Enterprise(9) button on each application in Apps (with a count of its log categories) opens enterprise9.html for that app's linked API set: one +/- accordion per log category with a Bootstrap grid of the newest 25 records; Expand/Collapse all." },
    { build: "4.5", title: "Log discovery", notes: "Every endpoint with \"log\" in its path is Enterprise(9). The Logs page builds each application's log sets from its registered endpoints (grouped by the log segment, e.g. /api/Apilog and /api/Apilog/{id}), ignores login/logout/catalog-style words and the host name, and offers the 12 standard sets with their routes (ApiLog, LunaLog, SysLog, UserLocation, UserProfileLog, LearnLog, Sessionlog, Superuserlog, Userhelp, Userlog, Usernotices, Usersession)." },
    { build: "4.4", title: "Enterprise(9) Logs", notes: "Logs page: pick an application to see the newest 25 records of each Enterprise(9) log (Apilog, Syslog, Adminlog, ...) read from its own API at /api/<Log>, with full detail per record. Log list in config.js. Logs button on Apps." },
    { build: "4.3", title: "Safe Audit host", notes: "Audit host and Sync Swagger check every existing endpoint (fresh from the database, matched by method + path however it was stored, or by operationId, on the same server) before inserting, so only genuinely new operations are added. Asks first, showing how many records will be inserted and linked. Unlinked matches are linked to the host instead of duplicated; existing duplicates are reported." },
    { build: "4.2", title: "Login", notes: "Sign-in page (login.html) with a hard-coded test user; every page checks the stored session (role auditor, userid not 901) and sends you to sign in otherwise. Sign out in the sidebar and on Home. Copyright notice on every page." },
    { build: "4.1", title: "Apps", notes: "Applications linked to one or more host APIs (ApplicationApi). Apps page with Link hosts; Dashboard shows each app's total endpoints across all its hosts; Endpoints can filter by app." },
    { build: "4.0", title: "Host Exceptions", notes: "Whole-host exceptions (ApiHostException): excepted hosts can't be audited; shown on Hosts, Run Audit, Exceptions, Dashboard and Home. Host details in their own Details dialog, copied to every endpoint when a host is audited." },
    { build: "3",   title: "AutoWalk of Swagger / Bug Fixes", notes: "One-click Audit host walks swagger.json, registers new operations with host defaults; Push details; required-field retry; date and URL fixes." },
    { build: "2",   title: "Home Page", notes: "Welcome page with live counts and the auditor graphic." },
    { build: "1",   title: "Base", notes: "Dashboard, Hosts, Endpoints, Run Audit, Exceptions and History pages on the audit API." }
  ],
  apiBaseUrl: "https://cockyanalyticsg4-cqfrgacteud3c2h3.westus3-01.azurewebsites.net",
  endpoints: {
    apiaudit: "/api/Apiaudit",
    apihosts: "/api/Apihosts",
    // Back-end interfaces/services registered behind an API (ApiInterfacesAudit table).
    // Change this if your controller route differs.
    apiinterfaces: "/api/ApiInterfacesAudit",
    apiauditresults: "/api/ApiAuditResults",
    auditexceptions: "/api/AuditExceptions",
    // Whole-host exceptions (ApiHostException table). Change this if your controller route differs.
    hostexceptions: "/api/ApiHostException",
    // Applications and the ApplicationApi link table (one app can use many hosts, one host can serve many apps)
    apps: "/api/Application",
    apphosts: "/api/ApplicationApi",
    // Welcome page top bar: Help (trouble tickets), Notices and Profile
    userhelp: "/api/Userhelp",
    usernotices: "/api/usernotices",
    userprofile: "/api/userprofile"
  },
  // Enterprise(9) logging standard. Any registered endpoint with "log" in its PATH is Enterprise(9), grouped into
  // sets by that path segment (/api/Apilog and /api/Apilog/{id} are the "Apilog" set). The Logs page finds them itself.
  // "types" below are the standard sets: they give tabs a label, and a set an app hasn't registered is still shown
  // (tried at GET <host>/api/<name>). Needed for sets without "log" in the name (Useraction, Usernotice, Usersession).
  // "path" overrides the route, "label" the tab name, "aliases" other route names for the same log.
  enterpriseLogs: {
    take: 25,   // newest records shown per log
    // Path words that contain "log" but aren't logs
    exclude: ["login", "logon", "logout", "logoff", "catalog", "dialog", "blog", "analog", "technolog", "backlog", "apolog"],
    types: [
      { name: "ApiLog",         path: "/api/ApiLog",         label: "API Log" },
      { name: "LunaLog",        path: "/api/LunaLog",        label: "Luna Log", aliases: ["AILunaLog"] },
      { name: "SysLog",         path: "/api/SysLog",         label: "Sys Log" },
      { name: "UserLocation",   path: "/api/userlocation/",  label: "User Location" },
      { name: "UserProfileLog", path: "/api/UserProfileLog", label: "User Profile Log" },
      { name: "LearnLog",       path: "/api/LearnLog",       label: "Learn Log" },
      { name: "Sessionlog",     path: "/api/Sessionlog",     label: "Session Log" },
      { name: "Superuserlog",   path: "/api/Superuserlog",   label: "Superuser Log" },
      { name: "Userhelp",       path: "/api/Userhelp",       label: "Trouble Tickets (Userhelp)" },
      { name: "Userlog",        path: "/api/Userlog",        label: "User Log" },
      { name: "Usernotice",     path: "/api/Usernotices",    label: "User Notices", aliases: ["Usernotices"] },
      { name: "Usersession",    path: "/api/Usersession",    label: "User Session" }
    ]
  },
  // How long to wait for the API before giving up (ms)
  requestTimeoutMs: 30000,
  // How long Run Audit waits for each endpoint being checked (ms)
  auditTimeoutMs: 15000
};
