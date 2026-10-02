// CockyAuditor configuration.
// This used to be config.json loaded with fetch(), which fails when the pages
// are opened straight from disk (file://). Keeping it as a script works both
// from disk and from a web server.
window.COCKY_CONFIG = {
  appName: "CockyAuditor",
  // Current build, shown on the home page and in the sidebar. Add each new build to the top of "builds".
  version: "5.8",
  // Copyright line shown on every page
  copyright: "\u00A9 2026-2027 CockyFinancial Services, Inc.",
  builds: [
    { build: "5.8", title: "Tickets, groups & customers", notes: "Dashboard: a Tickets section with stacked bars by type for Surveillance tickets (hard down / 5xx / very slow / other) and Customer tickets (by category), each split Open / In progress / Closed. Customers page under Users & Access (Customer table: add, edit, delete, with each customer's open / total customer tickets). User groups (/api/Usergroups): a Groups page under Users & Access (add, edit and delete groups, and pick each group's members) and, on Users, a Groups column, a group filter and a Groups button per user. A user's groups are stored in User.groupid1-5 (up to five) as the group's Groupid code; group owner (Groupownerid) and company (Groupcompanyid) are shown, and Findings note users in another company's group, slots naming groups that don't exist, and groups with no owner. Surveillance Tickets (WorkerTroubleTickets, the Operations Surveillance / Tier 2 queue) and Customer Troubles (the customer tickets in Userhelp, a read-only Bootstrap grid with status filter, search, paging and full detail per ticket; each ticket's customer name and tenant come from the Customer table, with a tenant filter) under Ops. Surveillance tickets: list, filter by status, add, edit, resolve (with resolution notes and root cause). New tickets fill Business unit and Application owner from the applications linked to the host, Evidence URL with the URL tested, and Auditor notes with the check's findings; Impacted users and Root cause are left for people. Repeats append to Auditor notes. Run Audit offers to open tickets for endpoints that are hard down (no response), return a 5xx server error, or are very slow (over performance.failMs); an endpoint that already has an open ticket gets its incident count raised instead of a second ticket. New Re-audit page under Ops: lists the endpoints whose last stamped result was hard down, 5xx or very slow, plus those with open tickets; re-checks just those, shows recovered vs still failing, stamps the new results, opens or updates tickets for the ones still failing and resolves tickets for the ones that recovered. Apps grid: Codebase and App OS columns. Hosts grid: Operating system and Hosted by (from OS_API and APIVendor on the host's linked applications, falling back to the host's OS type and a vendor guessed from the URL; flags apps that disagree)." },
    { build: "5.7", title: "App UI codebase", notes: "Applications record their UI codebase (UI_Codebase: mobile, html, ionic or react), which decides which of the vendor and operating system fields apply: HTML UI vendor and OS (UI_HTMLVendor, OS_UIHTML), React UI vendor and OS (UI_REACTVendor, OS_UIREACT), and the API vendor and OS (APIVendor, OS_API). Add and Edit application have a UI & platform section with a Codebase dropdown; the fields that don't apply to the chosen codebase are dimmed. The Apps grid has a UI & platform column and a codebase filter (apiapps.html?ui=react). Infra Audit checks each app: codebase not set or not in the list, missing vendor or OS for its codebase, details recorded for a UI it doesn't use, and end-of-support operating systems. The Dashboard has a fourth pie, Apps by UI codebase. Lists and rules in config.js appUi." },
    { build: "5.6", title: "Performance", notes: "New Performance page under Ops. Run Audit now saves each endpoint's response time and HTTP status when the run is saved (Apiaudit.LastResponseMs / LastStatusCode, with 'Also stamp' ticked). The page shows the last audit by default (or any of the last 15, or the latest time on every endpoint): median, 95th percentile, slowest, warning and slow counts; a D3 chart of the slowest 20 endpoints with the 2 s / 5 s thresholds; median and slowest per host; and a sortable table of every timed endpoint. Thresholds are in config.js performance and are used by Run Audit's findings too." },
    { build: "5.5", title: "Platform charts", notes: "D3 (v7) charts, in charts.js. The Dashboard opens with three pies: APIs, endpoints and apps by platform (DotNet vs NodeJS). A host's platform comes from its Framework version, Family, Database framework, repository and similar fields (rules in config.js platforms), then from its endpoints, and hosts with back-end interfaces from SystemConsole count as DotNet. Apps on both are Mixed. Hosts that can't be classified are listed under Unknown with a pointer to set Framework version." },
    { build: "5.4", title: "User roles & host fix", notes: "Users have three roles, set in Add user and Edit user: Role 1 (user, guest, admin, superuser), Role 2 (hrmanager, auditor, opsmanager, accountant) and Role 3 (SwaggerAdmin, none). The lists are in config.js roles. The Users grid has a Roles column and a role filter; Findings note superuser and SwaggerAdmin, and any stored role that isn't in its list. Fix: adding a host failed once the API's Apihost model gained navigation collections (ApplicationApis, ApiAccessPermissions); the add/edit dialog and Add API from Swagger now send those as empty lists instead of null or N/A." },
    { build: "5.3", title: "Users & Access", notes: "New Users & Access section. Users: add (through /api/Auth/signup, so the password is hashed), edit and delete users; passwords are never shown. Access (per user): pick the applications the user may use and their level (User.ApplicationIds / ApplicationPermissions), then API access for the host APIs of those applications only (ApiAccessPermissions), and site access with expiry and review (SiteAccessPermissions). Dropping an app removes API access no other app of the user covers. Sites page for the new Sites table. Access findings: API access outside the user's apps, apps or hosts that no longer exist or are inactive, expired or unreviewed site access, grants with no granted-by, and passwords stored or returned in plain text." },
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
    usersession: "/api/Usersession",
    // Users & Access (build 5.3)
    users: "/api/Users",
    authsignup: "/api/Auth/signup",            // new users are created here so the password is hashed
    sites: "/api/sites",
    apiaccess: "/api/apiaccesspermissions",
    siteaccess: "/api/siteaccesspermissions",
    // User groups (build 5.8)
    usergroups: "/api/Usergroups",
    // Trouble tickets (build 5.8)
    troubletickets: "/api/WorkerTroubleTickets",
    // Customer trouble tickets (build 5.8): the same Userhelp table the Welcome page's Help shows
    customertroubles: "/api/Userhelp",
    // Customers (Enterprise.Models.Customer: Id, Tenantid, UserId, FullName, CreatedAt), used to name the customer on each ticket.
    // The first route that answers is used.
    customers: "/api/Customers",
    customersAlt: "/api/Customer"
  },
  // API platform (build 5.4), for the Dashboard pies. A host is classified by the first rule whose regex matches the
  // text of its "fields" (then its endpoints' fields). Hosts whose back-end interfaces were read from SystemConsole are
  // "interfacesMean" (that console is .NET). Anything else is "Unknown": fill in Framework version on the host (Details).
  // Add a rule to chart another platform (e.g. { name: "Python", match: "python|django|flask|fastapi" }); colors follow rule order.
  platforms: {
    rules: [
      { name: "DotNet", match: "\\.net|dotnet|asp\\.?net|c#|csharp|entity ?framework|\\bnet ?(core|standard|framework|[4-9]|1\\d)\\b|\\biis\\b|kestrel" },
      { name: "NodeJS", match: "node|express|nestjs|next\\.?js|fastify|koa|javascript|typescript|\\bnpm\\b|\\bjs\\b|\\bts\\b" }
    ],
    fields: ["frameworkVersion", "family", "databaseFramework", "sourceRepository", "applicationName", "osType", "type"],
    interfacesMean: "DotNet",
    unknown: "Unknown",
    mixed: "Mixed"
  },
  // Application UI (build 5.7). Applications.UI_Codebase is the key field: it decides which vendor / OS fields apply.
  // fields: the Application property for each part (JSON name; case and underscores don't matter, so UI_Codebase,
  //   uI_Codebase and uiCodebase all match).
  // codebases: the values offered for UI_Codebase. uses: which UI field sets ("html", "react") that codebase needs.
  //   needs "any" = at least one of them must be filled (Ionic and mobile apps can be built on either).
  //   A value already stored that isn't listed is kept, shown, and flagged by Infra Audit.
  // vendors / operatingSystems: suggestions offered in those boxes (anything can still be typed).
  appUi: {
    fields: {
      codebase: "uI_Codebase",
      apiVendor: "apiVendor", apiOs: "oS_API",
      htmlVendor: "uI_HTMLVendor", htmlOs: "oS_UIHTML",
      reactVendor: "uI_REACTVendor", reactOs: "oS_UIREACT"
    },
    codebases: [
      { value: "mobile", label: "Mobile", uses: ["html", "react"], needs: "any", color: "#2a78d6" },
      { value: "html",   label: "HTML",   uses: ["html"],          needs: "all", color: "#eb6834" },
      { value: "ionic",  label: "Ionic",  uses: ["html", "react"], needs: "any", color: "#1baf7a" },
      { value: "react",  label: "React",  uses: ["react"],         needs: "all", color: "#4a3aa7" }
    ],
    vendors: ["Microsoft", "Meta", "Google", "Ionic", "Apple", "Vercel", "In-house"],
    // Suggestions for APIVendor: where the API is hosted
    apiVendors: ["Microsoft Azure", "AWS", "Google Cloud", "Oracle Cloud", "IBM Cloud", "DigitalOcean", "On-premises"],
    // Hosts screen: when no linked application records APIVendor, the hosting vendor is guessed from the host's URL
    // (and Azure subscription / resource group). Shown as "from URL"; set APIVendor on the app to replace the guess.
    hostingFromUrl: [
      { match: "azurewebsites\\.net|azure|cloudapp\\.net|windows\\.net", vendor: "Microsoft Azure" },
      { match: "amazonaws\\.com|awsapps|cloudfront\\.net|elasticbeanstalk", vendor: "AWS" },
      { match: "appspot\\.com|run\\.app|googleapis\\.com|cloudfunctions\\.net", vendor: "Google Cloud" },
      { match: "oraclecloud\\.com", vendor: "Oracle Cloud" },
      { match: "ondigitalocean\\.app", vendor: "DigitalOcean" }
    ],
    operatingSystems: ["Windows Server 2022", "Windows Server 2019", "Ubuntu 22.04", "Ubuntu 24.04", "RHEL 9", "iOS", "Android"],
    // Infra Audit: "warn" or "info" for a missing API vendor / OS (apps with no API details at all)
    missingApiLevel: "info"
  },
  // User roles (build 5.4). Each user has one value from each set, stored on the User record.
  // field: the User property (JSON name, any case). aliases: other names the API might use for it; the first one found
  // on the records is used. A value already stored that isn't in "values" is kept, shown, and noted in Findings.
  // required: false adds a "(not set)" choice. privileged: values noted in Findings (info) so they get a second look.
  roles: [
    { field: "role",  aliases: ["role1"], label: "Role 1", values: ["user", "guest", "admin", "superuser"], default: "user", required: true, privileged: ["superuser"] },
    { field: "role2", aliases: ["secondaryRole"], label: "Role 2", values: ["hrmanager", "auditor", "opsmanager", "accountant"], default: "", required: false },
    { field: "role3", aliases: ["tertiaryRole"], label: "Role 3", values: ["SwaggerAdmin", "none"], default: "none", required: true, privileged: ["SwaggerAdmin"] }
  ],
  // User groups (build 5.8). /api/Usergroups holds the groups (Enterprise.Models.Usergroup: Id, Groupid, Groupdescription,
  // Groupownerid, Groupcompanyid). A user's groups are the User fields in userSlots (up to five). Each slot stores the group's
  // Groupid code; a slot holding the group's numeric Id or its description is still recognised (and rewritten as the code
  // when that user's groups are next saved). companyCheck: note users in a group of another company (User.companyid vs Groupcompanyid).
  groups: {
    userSlots: ["groupid1", "groupid2", "groupid3", "groupid4", "groupid5"],
    companyCheck: true
  },
  // Users & Access (build 5.3)
  access: {
    // Choices offered for app, API and site access (any other value already stored is kept and shown)
    appLevels: ["Read", "Write", "Admin"],
    apiLevels: ["Read", "Write", "Admin"],
    siteLevels: ["Read", "Write", "Admin"],
    defaultLevel: "Read",
    // User.ApplicationIds / ApplicationPermissions are List<string>. "parallel": the permission for ApplicationIds[i]
    // is ApplicationPermissions[i]. "pairs": ApplicationPermissions holds "<appId>:<level>" entries.
    // Both are read either way; this decides how they're written.
    appPermissionFormat: "parallel",
    // Site access not reviewed within this many days is flagged
    reviewDays: 90
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
  auditTimeoutMs: 15000,
  // Response-time thresholds (ms), used by Run Audit's findings and the Performance page (build 5.6)
  performance: { warnMs: 2000, failMs: 5000, chartTop: 20 },
  // Trouble tickets (build 5.8, WorkerTroubleTickets). An endpoint is a problem when its check:
  //   down  - got no response at all (timeout, refused, DNS, TLS), or was stamped LastStatusCode 0  -> IsHardDown = 1
  //   error - answered with HTTP 500 or higher
  //   slow  - answered, but slower than performance.failMs
  // problems: severity given to each kind and whether Run Audit ticks it for a ticket by default.
  // A ticket is "open" while its Status isn't one of closedStatuses; a new problem on an endpoint with an open ticket
  // (same ApiName + Endpoint) raises that ticket's IncidentCount instead of opening another.
  tickets: {
    numberPrefix: "CA",            // TicketNumber = CA-20261002-153012-17 (date-time-endpoint id)
    statuses: ["Open", "In Progress", "Resolved", "Closed"],
    closedStatuses: ["Resolved", "Closed"],
    severities: ["Critical", "High", "Medium", "Low"],
    problems: {
      down:  { label: "Hard down",    severity: "Critical", ticketByDefault: true },
      error: { label: "Server error (5xx)", severity: "High", ticketByDefault: true },
      slow:  { label: "Very slow",    severity: "Medium",   ticketByDefault: false }
    },
    defaultEnvironment: "Unknown"   // Environment is required; used when neither the endpoint nor its host has one
  },
  // Customer Troubles page (build 5.8), read-only. Columns are picked from the records (id, date, subject, customer, status,
  // priority, then other short fields) unless "columns" lists them (JSON names, any case). A ticket is open unless its status
  // matches closedMatch. pageSizes: rows per page choices.
  // A ticket's customer: the ticket's user id field (userField, or a field named userid / customerid) matched against
  // Customer.UserId, then Customer.Id.
  customerTroubles: {
    columns: [],
    userField: "",
    closedMatch: "^(closed|resolved|done|complete|completed|cancel+ed|fixed)$",
    pageSizes: [25, 50, 100]
  }
};
