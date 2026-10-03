// Database Tables (builds 6.5 / 6.6): read and update the DatabaseTable inventory an API serves at <source>/api/databasetables.
// Shared by databasetables.html and report02.html. window.DbTables.
(function () {
  const CFG = window.COCKY_CONFIG;
  const PATH = CFG.endpoints.databasetables || "/api/databasetables";
  const trim = u => String(u || "").replace(/\/+$/, "");
  const me = () => window.CockyAuth?.current() || {};
  const UNASSIGNED = "Unassigned";

  // Schema Area: the JSON name the API uses for the SchemaArea column (schemaArea, SchemaArea, schema_area, ...)
  const areaKey = r => Object.keys(r || {}).find(k => k.toLowerCase().replace(/[^a-z]/g, "") === "schemaarea") || "schemaArea";
  const areaOf = r => { const v = r?.[areaKey(r)]; return v == null || String(v).trim() === "" ? "" : String(v).trim(); };
  // The configured list (config.js schemaAreas) plus any other value already in use
  const areas = (rows = []) => {
    const list = (CFG.schemaAreas || []).slice();
    rows.map(areaOf).filter(Boolean).forEach(a => { if (!list.some(x => x.toLowerCase() === a.toLowerCase())) list.push(a); });
    return list;
  };

  const baseOf = source => source ? trim(source) : trim(CFG.apiBaseUrl);
  const urlOf = (source, suffix = "") => baseOf(source) + PATH + suffix;

  async function call(method, url, body) {
    const ctrl = new AbortController(), t = setTimeout(() => ctrl.abort(), CFG.requestTimeoutMs || 30000);
    const headers = { Accept: "application/json" };
    if (body !== undefined) headers["Content-Type"] = "application/json";
    const tok = me().token;
    if (tok) headers.Authorization = "Bearer " + String(tok).replace(/^Bearer\s+/i, "");
    try {
      const res = await fetch(url, { method, headers, cache: "no-store", signal: ctrl.signal, body: body === undefined ? undefined : JSON.stringify(body) });
      const text = await res.text();
      if (!res.ok) { const e = new Error(`${res.status} ${res.statusText}${text && text.length < 300 ? " - " + text : ""}`); e.status = res.status; throw e; }
      let data = null; try { data = text ? JSON.parse(text) : null; } catch { data = text; }
      return data;
    } catch (e) {
      if (e.name === "AbortError") throw new Error("Timed out.");
      if (e instanceof TypeError) throw new Error(`Couldn't reach ${url} (CORS or the API is down). It must allow ${location.origin}${method !== "GET" ? ` and the ${method} method` : ""}.`);
      throw e;
    } finally { clearTimeout(t); }
  }

  async function list(source) {
    const data = await call("GET", urlOf(source));
    return Array.isArray(data) ? data : (data && typeof data === "object" ? [data] : []);
  }

  // Save a table's Schema Area: PUT <source>/api/databasetables/{id} with the whole record (the usual ASP.NET PUT),
  // then read it back, because some PUTs answer 2xx without saving a property they don't map.
  // Resolves { row, verified } where verified is true / false / null (couldn't read back).
  async function saveArea(source, row, area) {
    if (row.id == null) throw new Error("This table has no id, so it can't be updated.");
    const key = areaKey(row);
    const body = { ...row, [key]: area || null };
    if ("modifiedDate" in row) body.modifiedDate = new Date().toISOString();
    const saved = await call("PUT", urlOf(source, "/" + encodeURIComponent(row.id)), body);
    let back = null;
    try { back = await call("GET", urlOf(source, "/" + encodeURIComponent(row.id))); } catch { back = null; }
    if (back && typeof back === "object" && !Array.isArray(back)) {
      const ok = areaOf(back) === (area || "");
      return { row: ok ? back : { ...row, [key]: area || null }, verified: ok };
    }
    return { row: saved && typeof saved === "object" && !Array.isArray(saved) ? saved : { ...row, [key]: area || null }, verified: null };
  }

  // Sources: this API plus every host with a URL, sorted by name. [{ value, label, host }]
  function sources(hosts) {
    const hostName = h => h.apiHostName || h.apiHostUrl || ("#" + h.id);
    return [{ value: "", label: `This API (${new URL(CFG.apiBaseUrl).host})`, host: null },
      ...hosts.filter(h => h.apiHostUrl).sort((a, b) => hostName(a).localeCompare(hostName(b)))
        .map(h => ({ value: h.apiHostUrl, label: `${hostName(h)} · ${h.apiHostUrl}`, name: hostName(h), host: h }))];
  }

  const fullName = r => (r.schemaName ? r.schemaName + "." : "") + (r.tableName || "");

  // ---------- DatabaseFields (build 6.7) ----------
  const FPATH = CFG.endpoints.databasefields || "/api/databasefields";
  const fieldsUrl = (source, suffix = "") => baseOf(source) + FPATH + suffix;
  const asList = d => Array.isArray(d) ? d : (d && typeof d === "object" ? (Array.isArray(d.fields) ? d.fields : Array.isArray(d.items) ? d.items : [d]) : []);
  const fieldName = f => f.fieldName || f.columnName || f.name || "";
  const lc = v => String(v ?? "").trim().toLowerCase();
  // DatabaseTableId when both sides have it; otherwise the table names match and, if both have a schema, the schemas
  const fieldOf = (f, t) => f.databaseTableId != null && t.id != null ? String(f.databaseTableId) === String(t.id)
    : lc(f.tableName) === lc(t.tableName) && (!f.schemaName || !t.schemaName || lc(f.schemaName) === lc(t.schemaName));
  const fields = {
    PATH: FPATH, url: fieldsUrl, nameOf: fieldName, belongsTo: fieldOf,
    all: async source => asList(await call("GET", fieldsUrl(source))),
    // GET /table/{tableName}, narrowed to the table's schema when several schemas share a table name
    forTable: async (source, t) => asList(await call("GET", fieldsUrl(source, "/table/" + encodeURIComponent(t.tableName || "")))).filter(f => fieldOf(f, t)),
    create: (source, body) => call("POST", fieldsUrl(source), body),
    update: (source, id, body) => call("PUT", fieldsUrl(source, "/" + encodeURIComponent(id)), body),
    remove: (source, id) => call("DELETE", fieldsUrl(source, "/" + encodeURIComponent(id))),
    discover: source => call("GET", fieldsUrl(source, "/discovery"))
  };

  window.DbTables = { PATH, UNASSIGNED, areaKey, areaOf, areas, list, saveArea, sources, urlOf, fullName, fields };
})();
