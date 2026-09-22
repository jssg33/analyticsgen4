// CockyAuditor configuration.
// This used to be config.json loaded with fetch(), which fails when the pages
// are opened straight from disk (file://). Keeping it as a script works both
// from disk and from a web server.
window.COCKY_CONFIG = {
  appName: "CockyAuditor",
  // Current build, shown on the home page and in the sidebar. Add each new build to the top of "builds".
  version: "4.0",
  builds: [
    { build: "4.0", title: "Host Exceptions", notes: "Whole-host exceptions (ApiHostException): excepted hosts can't be audited; shown on Hosts, Run Audit, Exceptions, Dashboard and Home. Host details in their own Details dialog, copied to every endpoint when a host is audited." },
    { build: "3",   title: "AutoWalk of Swagger / Bug Fixes", notes: "One-click Audit host walks swagger.json, registers new operations with host defaults; Push details; required-field retry; date and URL fixes." },
    { build: "2",   title: "Home Page", notes: "Welcome page with live counts and the auditor graphic." },
    { build: "1",   title: "Base", notes: "Dashboard, Hosts, Endpoints, Run Audit, Exceptions and History pages on the audit API." }
  ],
  apiBaseUrl: "https://cockyanalyticsg4-cqfrgacteud3c2h3.westus3-01.azurewebsites.net",
  endpoints: {
    apiaudit: "/api/Apiaudit",
    apihosts: "/api/Apihosts",
    apiauditresults: "/api/ApiAuditResults",
    auditexceptions: "/api/AuditExceptions",
    // Whole-host exceptions (ApiHostException table). Change this if your controller route differs.
    hostexceptions: "/api/ApiHostException"
  },
  // How long to wait for the API before giving up (ms)
  requestTimeoutMs: 30000,
  // How long Run Audit waits for each endpoint being checked (ms)
  auditTimeoutMs: 15000
};
