// CockyAuditor configuration.
// This used to be config.json loaded with fetch(), which fails when the pages
// are opened straight from disk (file://). Keeping it as a script works both
// from disk and from a web server.
window.COCKY_CONFIG = {
  appName: "CockyAuditor",
  // Current build, shown on the home page and in the sidebar. Add each new build to the top of "builds".
  version: "5.2",
  // Copyright line shown on every page
  copyright: "\u00A9 2026-2027 CockyFinancial Services, Inc.",
  builds: [
    { build: "5.2", title: "API sign-in & Weather test", notes: "Sign-in now uses the API: login.html POSTs to /api/Auth/signin (falling back to /api/Auth/login) and keeps the returned token, user and role; pages send you back to sign in when the token expires, and Sign out ends the Usersession. audit / audit is still available as a test login and uses a fixed test token (config.js auth.testLogin). New Utilities section in the sidebar with a Weather page (weather.html) for the token-protected /api/weather test API. Sign in with /api/Auth/login (or paste a token, or pick a recent one from /api/Usersession) and Get forecast shows the 5-day forecast. Run all tests calls all 8 routes (GET/POST/PUT/DELETE, each with and without a token) plus the invalid-token cases, and checks each answers as designed: 400 FusionIdentity 1002 without a token, 400 1001 with a bad one, 200 with a valid one." },
    { build: "5.1", title: "Proxy servers", notes: "Web servers carry two new flags: IsProxy 1 = a proxy server; IsProxy 0 + IsProxySlave 1 = a backend web server sitting behind a proxy. The Dashboard and Welcome page split web servers into Web servers (backend) and Proxy servers, each with its own count and Infra Audit fail/warn totals, plus a note of servers whose role isn't set. Web Servers has a Role column and filter (webservers.html?role=slave|proxy|unset) and the edit form has a Proxy role section. Infra Audit labels each web server by role and warns when the role isn't set, when a backend server is public facing, when backend servers exist but no active proxy does, or when a server is flagged as both." },
    { build: "5.0", title: "Interface discovery", notes: "Walk Swagger & audit host now also reads the host API's SystemConsole and saves its back-end interface methods against the host (ApiInterfacesAudit.ApiHostId), asking first; re-audits never duplicate them, and methods no longer reported are marked not registered. The Interfaces page associates each method with one of the host's endpoints, with suggestions from the names (IPaymentService24.GetById24 -> GET /api/Payments/{id}). Run Audit shows each endpoint's back-end methods. Welcome page shows Infrastructure totals; the Apps grid has an endpoint totals row." },
    { build: "4.9", title: "Infrastructure", notes: "Inventory and audit of web servers, web farms (with their member servers), database servers and databases (WebServers, WebFarms, WebFarmServers, DatabaseServers, Databases). Each has its own page with a Findings column; Infra Audit checks all of them against the policy in config.js (TLS, end-of-support OS/platform versions, public exposure, backups, encryption of PII/PHI/PCI data, ownership). Applications can point to a web server, web farm and database." },
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
    userprofile: "/api/userprofile",
    // Infrastructure (build 4.9). A route can be a full https:// URL if these live on a different API than apiBaseUrl.
    webservers: "/api/webservers",
    webfarms: "/api/webfarms",
    webfarmservers: "/api/webfarmservers",   // link table: webFarmId + webServerId (POST/DELETE only, no PUT)
    databaseservers: "/api/databaseservers",
    databases: "/api/databases",
    // Utilities (build 5.2): weather test API. Every call needs a Usersessions token in the path (/api/weather/{token}).
    weather: "/api/weather",
    // Auth controller: POST { username, plainPassword } -> { userId, userUsername, userRole, token, sessionId, ... }
    // Used by the Weather page's "Sign in for a token"; the sign-in page uses auth.loginPaths below.
    authlogin: "/api/Auth/login",
    usersession: "/api/Usersession"
  },
  // Infra Audit policy. Every rule is checked on the Infra Audit page and summarised in each inventory page's Findings column.
  // Levels: "fail" or "warn". Version rules are matched (case-insensitive regex) against "<type/platform> <version>"
  // for servers and "<operating system>" for web servers. Support dates are as of September 2026; adjust to your policy.
  infraAudit: {
    minTlsVersion: 1.2,
    // Blank fields reported as warnings on every active item (names are the API's JSON names; case doesn't matter)
    requiredFields: ["owner", "supportTeam", "environment", "inventoryId"],
    endOfSupport: {
      os: [
        { match: "windows server 20(03|08)|windows server 2012|windows (xp|vista|7|8(\\.1)?|10)\\b", level: "fail", text: "Operating system is past end of support" },
        { match: "windows server 2016", level: "warn", text: "Windows Server 2016 extended support ends January 2027" },
        { match: "centos( linux)?\\s*[5-8]\\b|centos stream 8", level: "fail", text: "CentOS release is past end of life" },
        { match: "(rhel|red hat)[^0-9]*[4-6](\\.|\\b)", level: "fail", text: "RHEL release is past end of support" },
        { match: "(rhel|red hat)[^0-9]*7(\\.|\\b)", level: "warn", text: "RHEL 7 is in extended life support only" },
        { match: "ubuntu[^0-9]*(1[0-8])\\.(04|10)", level: "fail", text: "Ubuntu release is past end of standard support" },
        { match: "ubuntu[^0-9]*20\\.04", level: "warn", text: "Ubuntu 20.04 standard support ended May 2025 (ESM only)" }
      ],
      webserver: [
        { match: "apache\\S*\\s+(httpd\\s+)?2\\.[0-2](\\.|\\s|$)", level: "fail", text: "Apache httpd 2.2 or older is past end of life" },
        { match: "tomcat\\S*\\s+([5-7]|8\\.[05])(\\.|\\s|$)", level: "fail", text: "Tomcat release is past end of life" },
        { match: "iis\\S*\\s+([5-7](\\.\\d)?|8(\\.[05])?)(\\s|$)", level: "fail", text: "IIS version belongs to an unsupported Windows release" }
      ],
      database: [
        { match: "sql server\\s*(2000|2005|2008|2012|2014|2016)\\b", level: "fail", text: "SQL Server release is past extended support" },
        { match: "sql server\\s*2017\\b", level: "warn", text: "SQL Server 2017 extended support ends October 2027" },
        { match: "(sql server|mssql)\\D*\\b(8|9|10|11|12|13)\\.\\d", level: "fail", text: "SQL Server build number is a release past extended support (13.x = 2016 or older)" },
        { match: "(sql server|mssql)\\D*\\b14\\.\\d", level: "warn", text: "SQL Server 14.x (2017) extended support ends October 2027" },
        { match: "mysql\\D*(5\\.[0-7]|8\\.0)(\\.|\\s|$)", level: "fail", text: "MySQL release is past end of life" },
        { match: "postgre\\w*\\D*\\b([0-9]|1[0-3])(\\.\\d+)?(\\s|$)", level: "fail", text: "PostgreSQL major version is past end of life (13 or older)" }
      ]
    },
    // Classifications that must never hold PII/PHI/PCI data
    publicClassifications: ["public", "unclassified", "open"]
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
  // Back-end interfaces (build 5.0). While a host is audited, CockyAuditor reads <apiHostUrl> + each path below (first one that
  // answers wins) with the host's Swagger login, and saves the interfaces it lists against that host.
  // Expected answer: [{ "interfaceName": "IApilogService02", "methods": ["Create02", "GetAll02", ...] }, ...]
  // (implementationName / serviceLifetime are saved too if the console reports them). onAudit: false turns the step off.
  interfaceDiscovery: {
    onAudit: true,
    paths: ["/SystemConsole", "/api/SystemConsole"]
  },
  // Sign-in (build 5.2). login.html POSTs { username, plainPassword } to each path in turn until one isn't 404/405,
  // and stores the token it gets back. A path can be a full https:// URL.
  auth: {
    loginPaths: ["/api/Auth/signin", "/api/Auth/login"],
    // Best effort on Sign out: PUT <path>/{token}?id=-1 (marks the Usersession complete). "" turns it off.
    logoutPath: "/api/Users/logout",
    // Roles allowed into CockyAuditor, compared without case. Empty = any account that signs in.
    allowedRoles: [],
    // Test login: audit / audit signs in without calling the API and uses this token.
    // Anyone who can open config.js can read this token and use it against the API: turn it off (enabled: false)
    // before the site is public. Its JWT expiry isn't checked for the test login.
    testLogin: {
      enabled: true,
      username: "audit",
      password: "audit",
      token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqYW1lc0BnbG9jYXRpb24uaW5mbyIsImVtYWlsIjoiamFtZXNAZ2xvY2F0aW9uLmluZm8iLCJqdGkiOiIyOThmYTI3MS1lMzAzLTRlMDgtYjYzMy1hZmNiNmVmZWYwNTMiLCJyb2xlIjoicmVnaXN0ZXJlZCIsImV4cCI6MTc5MDU0NDg5NCwiaXNzIjoiaHR0cHM6Ly93d3cuNTkwdGVhbTEuaW5mbyIsImF1ZCI6Imh0dHBzOi8vd3d3LjU5MHRlYW0xLmluZm8ifQ.EBXbXsElT2OXJcMbrve-hAeO4CZETX9Dy0JOMMXEktY",
      session: { userid: "10000", role: "auditor", username: "auditor", email: "auditor@capitoltechnology.net", fullname: "Test Auditor" }
    }
  },
  // How long to wait for the API before giving up (ms)
  requestTimeoutMs: 30000,
  // How long Run Audit waits for each endpoint being checked (ms)
  auditTimeoutMs: 15000
};
