// CockyAuditor configuration.
// This used to be config.json loaded with fetch(), which fails when the pages
// are opened straight from disk (file://). Keeping it as a script works both
// from disk and from a web server.
window.COCKY_CONFIG = {
  appName: "CockyAuditor",
  apiBaseUrl: "https://cockyanalyticsg4-cqfrgacteud3c2h3.westus3-01.azurewebsites.net",
  endpoints: {
    apiaudit: "/api/Apiaudit",
    apihosts: "/api/Apihosts",
    apiauditresults: "/api/ApiAuditResults",
    auditexceptions: "/api/AuditExceptions"
  },
  // How long to wait for the API before giving up (ms)
  requestTimeoutMs: 30000,
  // How long Run Audit waits for each endpoint being checked (ms)
  auditTimeoutMs: 15000
};
