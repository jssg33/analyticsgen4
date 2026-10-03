// CockyAuditor Infrastructure (build 4.9): loading and auditing web servers, web farms, database servers and databases.
// Load after app.js. Policy (minimum TLS, end-of-support versions, required fields) is in config.js -> infraAudit.
//
// Audit = checking each inventory record against the policy. Nothing is contacted: the checks read the records only.
(function () {
  const { api, esc, CFG } = window.Cocky;
  const POLICY = CFG.infraAudit || {};

  // ---------- reading records ----------
  // Field lookup that ignores case, so the checks work whether the API returns tlsVersion, TLSVersion or TlsVersion
  function pick(r, name) {
    if (!r) return undefined;
    if (name in r) return r[name];
    const lower = name.toLowerCase();
    let k = Object.keys(r).find(x => x.toLowerCase() === lower);
    // then ignoring underscores too (UI_Codebase, uI_Codebase, uiCodebase)
    if (k === undefined) { const n = lower.replace(/[^a-z0-9]/g, ""); k = Object.keys(r).find(x => x.toLowerCase().replace(/[^a-z0-9]/g, "") === n); }
    return k === undefined ? undefined : r[k];
  }
  // Blank, or one of the placeholders Swagger / the required-field retry leave behind
  const blank = v => v == null || (typeof v === "string" && ["", "string", "n/a", "na", "none", "null", "-"].includes(v.trim().toLowerCase()));
  const str = (r, name) => { const v = pick(r, name); return blank(v) ? "" : String(v).trim(); };
  const yes = (r, name, dflt = false) => { const v = pick(r, name); return v == null ? dflt : v === true || v === 1 || /^(true|1|yes)$/i.test(String(v)); };
  const num = (r, name) => { const v = pick(r, name); return v == null || v === "" ? null : Number(v); };

  // ---------- the kinds of record ----------
  const KINDS = {
    webservers:      { label: "Web server",      plural: "Web servers",      page: "webservers.html", activeField: "active",
                       name: r => str(r, "serverName") || str(r, "hostname") || "#" + r.id },
    webfarms:        { label: "Web farm",        plural: "Web farms",        page: "webfarms.html",   activeField: "enabled",
                       name: r => str(r, "farmName") || "#" + r.id },
    databaseservers: { label: "Database server", plural: "Database servers", page: "dbservers.html",  activeField: "active",
                       name: r => str(r, "serverName") || str(r, "hostname") || "#" + r.id },
    databases:       { label: "Database",        plural: "Databases",        page: "databases.html",  activeField: "active",
                       name: r => str(r, "databaseName") || "#" + r.id },
    apps:            { label: "Application",     plural: "Applications",     page: "apiapps.html",    activeField: "isActive",
                       name: r => str(r, "applicationName") || "#" + r.id }
  };
  const isActive = (kind, r) => yes(r, KINDS[kind].activeField, true);

  // Web server roles (build 5.1):
  //   isProxy 1                  = a proxy server
  //   isProxy 0, isProxySlave 1  = a backend web server (a regular web server behind a proxy): the "Web servers"
  //   neither flag set (null/0)  = role not set yet; Infra Audit warns until one is ticked
  // A server flagged as both counts as a proxy (and gets a warning).
  const WEB_ROLES = {
    slave:  { label: "Web server",    plural: "Web servers",   tag: "Backend",      badge: "text-bg-light border" },
    proxy:  { label: "Proxy server",  plural: "Proxy servers", tag: "Proxy",        badge: "text-bg-primary" },
    unset:  { label: "Web server (role not set)", plural: "Role not set", tag: "Role not set", badge: "text-bg-warning" }
  };
  const webRole = r => yes(r, "isProxy") ? "proxy" : yes(r, "isProxySlave") ? "slave" : "unset";

  // ---------- loading ----------
  // Loads every infrastructure table plus applications. A route that answers 404 isn't deployed yet: it comes back as
  // an empty list and is named in `missing`. Other errors are collected in `errors` (the rest still loads).
  async function loadAll(which = ["webservers", "webfarms", "webfarmservers", "databaseservers", "databases", "apps"]) {
    const out = { missing: [], errors: [] };
    await Promise.all(which.map(r => api.list(r)
      .then(rows => { out[r] = rows; })
      .catch(e => { out[r] = []; (e.status === 404 ? out.missing : out.errors).push(e.status === 404 ? r : e); })));
    return out;
  }

  // Before building a page on one of these routes: returns false and shows a setup note if the API doesn't have it yet
  async function guard(resource, mount) {
    try { await api.list(resource); return true; }
    catch (e) {
      if (e.status !== 404) return true;   // let the page show the real error with Retry
      mount.insertAdjacentHTML("beforeend", `<div class="alert alert-warning">
        <strong>The API didn't answer on <code>${esc(CFG.endpoints[resource])}</code>.</strong>
        Check that the ${esc(KINDS[resource]?.plural?.toLowerCase() || resource)} endpoints are deployed on
        <code>${esc(CFG.apiBaseUrl)}</code>, or set the route in <code>config.js</code> (it can be a full URL if they live on another API).
        <div class="small mt-2 text-muted">${esc(e.message)}</div></div>`);
      return false;
    }
  }

  // ---------- relationships ----------
  function index(d) {
    const byId = rows => new Map((rows || []).map(r => [r.id, r]));
    const ws = byId(d.webservers), wf = byId(d.webfarms), dbs = byId(d.databaseservers), db = byId(d.databases);
    const links = (d.webfarmservers || []).map(l => ({ id: l.id, farmId: num(l, "webFarmId"), serverId: num(l, "webServerId") }));
    const membersOf = farmId => [...new Set(links.filter(l => l.farmId === farmId).map(l => l.serverId))];
    const farmsOf = serverId => [...new Set(links.filter(l => l.serverId === serverId).map(l => l.farmId))];
    const databasesOn = serverId => (d.databases || []).filter(x => num(x, "databaseServerId") === serverId);
    const appsUsing = (field, id) => (d.apps || []).filter(a => num(a, field) === id);
    // Same server added to the same farm more than once (the table has no unique constraint)
    const duplicateLinks = farmId => {
      const seen = new Map();
      links.filter(l => l.farmId === farmId).forEach(l => seen.set(l.serverId, (seen.get(l.serverId) || 0) + 1));
      return [...seen].filter(([, n]) => n > 1).map(([sid]) => sid);
    };
    // Applications only link to databases once the API returns databaseId on Application
    const appsHaveDatabase = (d.apps || []).some(a => pick(a, "databaseId") !== undefined);
    const proxies = (d.webservers || []).filter(r => webRole(r) === "proxy");
    const proxySlaves = (d.webservers || []).filter(r => webRole(r) === "slave");
    return { ws, wf, dbs, db, links, proxies, proxySlaves, membersOf, farmsOf, databasesOn, appsUsing, duplicateLinks, appsHaveDatabase };
  }

  // ---------- checks ----------
  // TLS: "1.2", "TLS 1.2", "TLSv1.3", "1.0/1.1/1.2"... The lowest version listed is what's allowed.
  function tlsFindings(r, add) {
    const raw = str(r, "tlsVersion");
    const min = POLICY.minTlsVersion || 1.2;
    if (!raw) return add("warn", "TLS version not recorded");
    if (/ssl/i.test(raw)) return add("fail", `Allows SSL (${raw}); minimum is TLS ${min}`);
    const vs = (raw.match(/\d(\.\d)?/g) || []).map(Number).filter(v => v >= 1 && v < 2);
    if (!vs.length) return add("warn", `TLS version "${raw}" not understood`);
    const low = Math.min(...vs);
    if (low < min) add("fail", `Allows TLS ${low.toFixed(1)}; minimum is TLS ${min}`);
  }
  function versionFindings(text, rules, add) {
    if (!text) return;
    (rules || []).forEach(rule => { try { if (new RegExp(rule.match, "i").test(text)) add(rule.level, `${rule.text} (${text})`); } catch { /* bad regex in config */ } });
  }
  function urlFindings(r, fields, publicFacing, add) {
    fields.forEach(f => {
      const u = str(r, f);
      if (/^http:\/\//i.test(u)) add(publicFacing ? "fail" : "warn", `${titleOf(f)} is plain HTTP: ${u}`);
    });
  }
  function requiredFindings(r, add, fields = POLICY.requiredFields || []) {
    const missing = fields.filter(f => !str(r, f));
    if (missing.length) add("warn", `Not recorded: ${missing.map(titleOf).join(", ")}`);
  }
  const titleOf = k => window.Cocky.titleCase(k).replace(/^Tls/, "TLS").replace(/U R L$|Url$/, "URL").replace(/^C Name/, "CNAME ").replace(/I P$/, "IP");
  const isProd = env => /^prod/i.test(env || "");
  const sensitive = r => ["PII", "PHI", "PCI"].filter(t => yes(r, "contains" + t));

  function checkWebServer(r, ix, add) {
    tlsFindings(r, add);
    versionFindings(str(r, "operatingSystem"), POLICY.endOfSupport?.os, add);
    versionFindings([str(r, "webServerType"), str(r, "version")].filter(Boolean).join(" "), POLICY.endOfSupport?.webserver, add);
    if (!str(r, "operatingSystem")) add("warn", "Operating system not recorded");
    if (!str(r, "webServerType") || !str(r, "version")) add("warn", "Web server type/version not recorded");
    urlFindings(r, ["applicationURL", "azureURL"], yes(r, "publicFacing"), add);
    if (!str(r, "hostname") && !str(r, "ipAddress")) add("warn", "No hostname or IP address");
    requiredFindings(r, add);
    // Proxies and the servers behind them
    const role = webRole(r);
    if (yes(r, "isProxy") && yes(r, "isProxySlave")) add("warn", "Marked as both a proxy and a proxy slave");
    if (role === "unset") add("warn", "Proxy role not set: tick Is Proxy (proxy server) or Is Proxy Slave (backend web server behind a proxy)");
    if (role === "slave") {
      if (yes(r, "publicFacing")) add("warn", "Backend web server is public facing; it should only be reached through its proxy");
      if (!ix.proxies.some(p => isActive("webservers", p))) add("warn", "Backend web server (behind a proxy), but no active proxy server is recorded");
    }
    if (role === "proxy" && !ix.proxySlaves.some(p => isActive("webservers", p))) add("info", "Proxy server, but no active backend web servers are recorded");
    ix.farmsOf(r.id).forEach(fid => {
      const f = ix.wf.get(fid); if (!f) return;
      const fe = str(f, "environment"), se = str(r, "environment");
      if (fe && se && fe.toLowerCase() !== se.toLowerCase()) add("warn", `Environment ${se} differs from its farm "${KINDS.webfarms.name(f)}" (${fe})`);
      if (yes(f, "publicFacing") && !yes(r, "publicFacing")) add("info", `Serves public traffic through farm "${KINDS.webfarms.name(f)}"`);
    });
  }

  function checkWebFarm(r, ix, add) {
    tlsFindings(r, add);
    const members = ix.membersOf(r.id);
    const known = members.map(id => ix.ws.get(id)).filter(Boolean);
    if (!members.length) add("fail", "Enabled farm has no member servers");
    const missing = members.filter(id => !ix.ws.has(id));
    if (missing.length) add("warn", `Member link points to a web server that doesn't exist (#${missing.join(", #")})`);
    const inactive = known.filter(s => !isActive("webservers", s));
    if (inactive.length) add("warn", `Inactive member server(s): ${inactive.map(KINDS.webservers.name).join(", ")}`);
    const pool = num(r, "backendPoolCount");
    if (pool != null && members.length && pool !== members.length) add("warn", `Backend pool count is ${pool} but ${members.length} server(s) are linked`);
    const dups = ix.duplicateLinks(r.id);
    if (dups.length) add("warn", `Server linked more than once: ${dups.map(id => ix.ws.get(id) ? KINDS.webservers.name(ix.ws.get(id)) : "#" + id).join(", ")}`);
    const env = str(r, "environment");
    const offEnv = env ? known.filter(s => str(s, "environment") && str(s, "environment").toLowerCase() !== env.toLowerCase()) : [];
    if (offEnv.length) add("warn", `Member server(s) in a different environment: ${offEnv.map(s => `${KINDS.webservers.name(s)} (${str(s, "environment")})`).join(", ")}`);
    const oldTls = known.filter(s => { const t = str(s, "tlsVersion"); return t && ((t.match(/\d(\.\d)?/g) || []).map(Number).some(v => v >= 1 && v < (POLICY.minTlsVersion || 1.2)) || /ssl/i.test(t)); });
    if (oldTls.length) add("fail", `Member server(s) allow old TLS: ${oldTls.map(KINDS.webservers.name).join(", ")}`);
    if (!str(r, "healthProbeUrl")) add("warn", "No health probe URL");
    if (!str(r, "loadBalancerName")) add("warn", "Load balancer not recorded");
    urlFindings(r, ["url", "applicationURL", "azureURL", "healthProbeUrl"], yes(r, "publicFacing"), add);
    requiredFindings(r, add);
  }

  function checkDatabaseServer(r, ix, add) {
    const dbs = ix.databasesOn(r.id).filter(x => isActive("databases", x));
    if (yes(r, "publicFacing")) add("fail", "Database server is public facing");
    versionFindings([str(r, "databasePlatform"), str(r, "version")].filter(Boolean).join(" "), POLICY.endOfSupport?.database, add);
    if (!str(r, "databasePlatform") || !str(r, "version")) add("warn", "Platform/version not recorded");
    const needBackup = dbs.filter(x => yes(x, "backupRequired", true));
    if (!str(r, "backupSolution")) add(needBackup.length ? "fail" : "warn",
      needBackup.length ? `No backup solution, but ${needBackup.length} database(s) on it require backup` : "No backup solution recorded");
    if (/^simple$/i.test(str(r, "recoveryModel")) && needBackup.length) add("warn", "Recovery model SIMPLE: no point-in-time restore for databases that require backup");
    if (!dbs.length) add("info", "No active databases recorded on this server");
    if (!str(r, "hostname") && !str(r, "ipAddress")) add("warn", "No hostname or IP address");
    requiredFindings(r, add);
  }

  function checkDatabase(r, ix, add) {
    const sid = num(r, "databaseServerId");
    const server = ix.dbs.get(sid);
    const data = sensitive(r);
    if (!server) add("fail", `Database server #${sid ?? "?"} doesn't exist`);
    else {
      if (!isActive("databaseservers", server)) add("warn", `Its server "${KINDS.databaseservers.name(server)}" is inactive`);
      if (yes(r, "backupRequired", true) && !str(server, "backupSolution")) add("fail", `Backup required, but server "${KINDS.databaseservers.name(server)}" has no backup solution`);
      if (data.length && yes(server, "publicFacing")) add("fail", `Holds ${data.join("/")} on a public-facing server`);
    }
    if (data.length && !yes(r, "encryptionEnabled")) add("fail", `Holds ${data.join("/")} but encryption is off`);
    const cls = str(r, "classification");
    if (!cls) add(data.length ? "fail" : "warn", data.length ? `Holds ${data.join("/")} but has no classification` : "Classification not recorded");
    else if (data.length && (POLICY.publicClassifications || []).includes(cls.toLowerCase())) add("fail", `Classified "${cls}" but holds ${data.join("/")}`);
    requiredFindings(r, add, ["owner", "supportTeam", "inventoryId"]);
    if (ix.appsHaveDatabase && !ix.appsUsing("databaseId", r.id).length) add("info", "No application points to this database");
  }

  function checkApp(r, ix, add) {
    const wsId = num(r, "webServerId"), wfId = num(r, "webFarmId"), dbId = num(r, "databaseId");
    const ref = (id, map, kind) => {
      if (!id) return;
      const x = map.get(id);
      if (!x) add("warn", `${KINDS[kind].label} #${id} doesn't exist`);
      else if (!isActive(kind, x)) add("warn", `Runs on ${KINDS[kind].label.toLowerCase()} "${KINDS[kind].name(x)}", which is ${kind === "webfarms" ? "disabled" : "inactive"}`);
      return x;
    };
    const ws = ref(wsId, ix.ws, "webservers"), wf = ref(wfId, ix.wf, "webfarms"), db = ref(dbId, ix.db, "databases");
    if (!wsId && !wfId) add("info", "No web server or web farm recorded");
    if (wsId && wfId && !ix.membersOf(wfId).includes(wsId)) add("warn", "Web server isn't a member of the app's web farm");
    const env = str(r, "environment");
    [[ws, "webservers"], [wf, "webfarms"]].forEach(([x, kind]) => {
      const xe = x && str(x, "environment");
      if (env && xe && xe.toLowerCase() !== env.toLowerCase()) add("warn", `App is ${env} but ${KINDS[kind].label.toLowerCase()} "${KINDS[kind].name(x)}" is ${xe}`);
    });
    if (db && env) {
      const srv = ix.dbs.get(num(db, "databaseServerId"));
      const se = srv && str(srv, "environment");
      if (se && se.toLowerCase() !== env.toLowerCase()) add("warn", `App is ${env} but its database server is ${se}`);
    }
    appUiFindings(r, add);
  }

  // ---------- application UI (build 5.7) ----------
  // UI_Codebase decides which vendor / OS fields apply (config.js appUi). Checked only once the API returns UI_Codebase.
  const UI = Object.assign({ fields: {}, codebases: [] }, CFG.appUi || {});
  const UI_SETS = {
    html:  { label: "HTML UI",  vendor: "htmlVendor",  os: "htmlOs" },
    react: { label: "React UI", vendor: "reactVendor", os: "reactOs" }
  };
  const uiField = (r, part) => str(r, UI.fields[part] || part);
  const hasUiFields = r => pick(r, UI.fields.codebase || "uI_Codebase") !== undefined;
  const codebaseOf = r => {
    const v = uiField(r, "codebase");
    return { value: v, def: v ? UI.codebases.find(c => c.value.toLowerCase() === v.toLowerCase()) || null : null };
  };
  // The app's own operating system(s): the UI OS for its codebase. Returns [{ set: "html"|"react", label, os, vendor }]
  // (for mobile / ionic, every UI set that has an OS; for an unknown codebase, both).
  function appOs(r) {
    const { def } = codebaseOf(r);
    const sets = def ? def.uses || [] : Object.keys(UI_SETS);
    return sets.map(s => ({ set: s, label: UI_SETS[s].label, os: uiField(r, UI_SETS[s].os), vendor: uiField(r, UI_SETS[s].vendor) }))
      .filter(x => def && def.needs !== "any" ? true : x.os || x.vendor);
  }
  // Hosts screen: a host API's operating system and hosting vendor, from OS_API / APIVendor on the applications linked to it
  // (ApplicationApi). Falls back to the host's own osType, and to a vendor guessed from its URL (config.js appUi.hostingFromUrl).
  // Returns { os: [{ value, apps: [names] }], vendor: [...], osFrom, vendorFrom: "apps"|"host"|"url"|"" , apps: [...] }
  function hostPlatform(host, apps, links) {
    const mine = (apps || []).filter(a => (links || []).some(l => l.applicationId === a.id && l.apiHostId === host.id));
    const group = part => {
      const m = new Map();
      mine.forEach(a => { const v = uiField(a, part); if (v) { const k = v.toLowerCase(); if (!m.has(k)) m.set(k, { value: v, apps: [] }); m.get(k).apps.push(str(a, "applicationName") || "#" + a.id); } });
      return [...m.values()];
    };
    let os = group("apiOs"), vendor = group("apiVendor"), osFrom = os.length ? "apps" : "", vendorFrom = vendor.length ? "apps" : "";
    if (!os.length && str(host, "osType")) { os = [{ value: str(host, "osType"), apps: [] }]; osFrom = "host"; }
    if (!vendor.length) {
      const text = [str(host, "apiHostUrl"), str(host, "fqdn"), str(host, "hostName"), str(host, "azureResourceGroup")].join(" ");
      let hit = (UI.hostingFromUrl || []).find(x => { try { return new RegExp(x.match, "i").test(text); } catch { return false; } });
      if (!hit && str(host, "azureSubscriptionId") && !/^0{8}-/.test(str(host, "azureSubscriptionId"))) hit = { vendor: "Microsoft Azure" };
      if (hit) { vendor = [{ value: hit.vendor, apps: [] }]; vendorFrom = "url"; }
    }
    return { os, vendor, osFrom, vendorFrom, apps: mine };
  }

  function appUiFindings(r, add) {
    if (!hasUiFields(r)) return;
    const { value, def } = codebaseOf(r);
    const missing = set => ["vendor", "os"].filter(p => !uiField(r, UI_SETS[set][p])).map(p => p === "vendor" ? "vendor" : "operating system");
    const filled = set => !missing(set).length;
    const anyOf = set => missing(set).length < 2;
    if (!value) add("warn", "UI codebase not set (mobile, html, ionic or react)");
    else if (!def) add("warn", `UI codebase "${value}" isn't one of ${UI.codebases.map(c => c.value).join(", ")}`);
    else {
      const uses = def.uses || [];
      if (def.needs === "any") {
        if (!uses.some(filled)) {
          const part = uses.filter(anyOf);
          add("warn", part.length ? `${def.label} app: ${UI_SETS[part[0]].label} ${missing(part[0]).join(" and ")} not recorded`
            : `${def.label} app: no UI vendor or operating system recorded (${uses.map(s => UI_SETS[s].label).join(" or ")})`);
        }
      } else uses.forEach(s => { const m = missing(s); if (m.length) add("warn", `${def.label} app: ${UI_SETS[s].label} ${m.join(" and ")} not recorded`); });
      Object.keys(UI_SETS).filter(s => !uses.includes(s) && anyOf(s))
        .forEach(s => add("info", `${UI_SETS[s].label} details are recorded but the codebase is ${def.label}`));
    }
    const apiMissing = ["apiVendor", "apiOs"].filter(p => !uiField(r, p)).map(p => p === "apiVendor" ? "vendor" : "operating system");
    if (apiMissing.length) add(UI.missingApiLevel || "info", `API ${apiMissing.join(" and ")} not recorded`);
    [["apiOs", "API OS"], ["htmlOs", "HTML UI OS"], ["reactOs", "React UI OS"]].forEach(([p, lbl]) =>
      versionFindings(uiField(r, p), POLICY.endOfSupport?.os, (lvl, text) => add(lvl, `${lbl}: ${text}`)));
  }

  const CHECKS = { webservers: checkWebServer, webfarms: checkWebFarm, databaseservers: checkDatabaseServer, databases: checkDatabase, apps: checkApp };
  const RANK = { fail: 3, warn: 2, info: 1 };

  // Audit everything in `d` (from loadAll). Returns { results: [...], get(kind, id), summary: { kind: { total, fail, warn, pass, inactive } } }.
  // A result: { kind, id, record, name, environment, owner, active, status: fail|warn|pass|inactive, findings: [{ level, text }] }
  function evaluate(d) {
    const ix = index(d);
    const results = [];
    for (const kind of Object.keys(CHECKS)) {
      for (const r of d[kind] || []) {
        const findings = [];
        const add = (level, text) => findings.push({ level, text });
        const active = isActive(kind, r);
        if (active) CHECKS[kind](r, ix, add);
        findings.sort((a, b) => RANK[b.level] - RANK[a.level]);
        const worst = findings.reduce((m, f) => Math.max(m, RANK[f.level]), 0);
        results.push({
          kind, id: r.id, record: r, name: KINDS[kind].name(r),
          environment: str(r, "environment"), owner: str(r, "owner") || str(r, "ownerName"), active,
          status: !active ? "inactive" : worst === 3 ? "fail" : worst === 2 ? "warn" : "pass",
          findings
        });
      }
    }
    const map = new Map(results.map(x => [x.kind + ":" + x.id, x]));
    const summary = {};
    for (const kind of Object.keys(CHECKS)) {
      const rs = results.filter(x => x.kind === kind);
      summary[kind] = { total: rs.length, fail: 0, warn: 0, pass: 0, inactive: 0 };
      rs.forEach(x => summary[kind][x.status]++);
    }
    // Web servers split by role: summary.webRoles.slave (backend) / .proxy / .unset, same shape as the others
    summary.webRoles = {};
    for (const role of Object.keys(WEB_ROLES)) {
      const rs = results.filter(x => x.kind === "webservers" && webRole(x.record) === role);
      summary.webRoles[role] = { total: rs.length, fail: 0, warn: 0, pass: 0, inactive: 0 };
      rs.forEach(x => summary.webRoles[role][x.status]++);
    }
    return { results, get: (kind, id) => map.get(kind + ":" + id), summary, index: ix };
  }

  // ---------- display ----------
  const STATUS_BADGE = { fail: "text-bg-danger", warn: "text-bg-warning", pass: "text-bg-success", inactive: "text-bg-secondary" };
  const LEVEL_ICON = { fail: "✖", warn: "▲", info: "ℹ" };
  // Findings column: "2 fail · 1 warn" badges, hover for the list, click to open Infra Audit on that record
  function badge(result) {
    if (!result) return '<span class="text-muted">—</span>';
    if (result.status === "inactive") return '<span class="badge text-bg-secondary fw-normal">not audited (inactive)</span>';
    const n = l => result.findings.filter(f => f.level === l).length;
    const tip = result.findings.filter(f => f.level !== "info").map(f => `${LEVEL_ICON[f.level]} ${f.text}`).join("\n") || "No problems found";
    const parts = [n("fail") && `<span class="badge text-bg-danger">${n("fail")} fail</span>`, n("warn") && `<span class="badge text-bg-warning">${n("warn")} warn</span>`].filter(Boolean);
    return `<a href="infraaudit.html?kind=${result.kind}&id=${encodeURIComponent(result.id)}" class="text-decoration-none text-nowrap" title="${esc(tip)}">${parts.join(" ") || '<span class="badge text-bg-success">OK</span>'}</a>`;
  }
  const findingList = fs => fs.length
    ? `<ul class="finding-list small">${fs.map(f => `<li class="lvl-${f.level}">${LEVEL_ICON[f.level]} <span class="text-body">${esc(f.text)}</span></li>`).join("")}</ul>`
    : '<span class="small text-success">No problems found</span>';

  const roleBadge = r => { const x = WEB_ROLES[webRole(r)]; return `<span class="badge ${x.badge} fw-normal">${x.tag}</span>`; };

  window.Infra = { appOs, hostPlatform, UI, UI_SETS, uiField, hasUiFields, codebaseOf, WEB_ROLES, webRole, roleBadge, KINDS, pick, str, yes, num, blank, loadAll, guard, index, evaluate, badge, findingList, STATUS_BADGE, LEVEL_ICON, sensitive, isActive };
})();
