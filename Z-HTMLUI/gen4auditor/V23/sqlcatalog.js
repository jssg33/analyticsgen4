// CockyAuditor SQL catalog (build 5.9): AuditSqlServers -> AuditSqlTables -> AuditSqlFields.
// Loading, findings, and "Import from SQL" (parse a pasted query result, then create the missing tables and columns).
// Load after app.js. Settings: config.js sqlCatalog; end-of-support version rules: config.js infraAudit.endOfSupport.database.
(function () {
  const { api, esc, CFG } = window.Cocky;
  const SC = Object.assign({ databaseTypes: ["MSSQL", "Oracle", "MySQL", "PostgreSQL"], phi: "", pii: "", unencryptedPhi: "fail", unencryptedPii: "warn", importBatch: 4 }, CFG.sqlCatalog || {});
  const PHI = SC.phi ? new RegExp(SC.phi, "i") : null, PII = SC.pii ? new RegExp(SC.pii, "i") : null;
  const lc = v => String(v ?? "").trim().toLowerCase();
  const pick = (o, name) => { if (!o) return undefined; if (name in o) return o[name]; const n = lc(name).replace(/_/g, ""); const k = Object.keys(o).find(x => lc(x).replace(/_/g, "") === n); return k === undefined ? undefined : o[k]; };
  const str = (o, n) => { const v = pick(o, n); return v == null || ["", "string"].includes(lc(v)) ? "" : String(v).trim(); };
  const yes = (o, n) => { const v = pick(o, n); return v === true || v === 1 || /^(true|1|yes|y)$/i.test(String(v ?? "")); };
  const num = (o, n) => { const v = pick(o, n); return v == null || v === "" ? null : Number(v); };

  // ---------- loading ----------
  async function loadAll() {
    const out = { missing: [] };
    await Promise.all(["sqlservers", "sqltables", "sqlfields"].map(k => api.list(k).then(r => { out[k] = r; })
      .catch(e => { out[k] = []; if (e.status === 404) out.missing.push(k); else throw e; })));
    return index(out);
  }
  function index(d) {
    d.serverById = new Map(d.sqlservers.map(s => [s.id, s]));
    d.tableById = new Map(d.sqltables.map(t => [t.id, t]));
    d.tablesOf = id => d.sqltables.filter(t => num(t, "sqlServerId") === id);
    d.fieldsOf = id => d.sqlfields.filter(f => num(f, "sqlTableId") === id);
    return d;
  }
  const tableName = t => `${str(t, "schemaName") || "dbo"}.${str(t, "tableName")}`;
  const serverName = s => s ? `${str(s, "serverName")} / ${str(s, "databaseName")}` : "";

  // ---------- name-based PII / PHI suggestions ----------
  const guess = name => ({ phi: !!(PHI && PHI.test(name)), pii: !!(PII && PII.test(name)) });

  // ---------- findings ----------
  // Each returns [{ level: fail|warn|info, text }]
  function fieldFindings(f) {
    const out = [], add = (level, text) => out.push({ level, text });
    const phi = yes(f, "containsPHI"), pii = yes(f, "containsPII"), enc = yes(f, "isEncrypted"), name = str(f, "columnName");
    const masked = !!str(f, "maskingRule");
    if (phi && !enc) add(SC.unencryptedPhi, `PHI column isn't encrypted${masked ? " (masking hides it from some users, but the data isn't encrypted)" : ""}`);
    else if (pii && !enc) add(masked ? "info" : SC.unencryptedPii, masked ? "PII column isn't encrypted (masked)" : "PII column isn't encrypted");
    if ((phi || pii) && !str(f, "sensitivityLabel")) add("info", "Sensitive column has no sensitivity label");
    const g = guess(name);
    if (g.phi && !phi) add("info", "Name looks like PHI but it isn't flagged");
    else if (g.pii && !pii && !phi) add("info", "Name looks like PII but it isn't flagged");
    if ((phi || pii) && !str(f, "description")) add("info", "Sensitive column has no description");
    return out;
  }
  function tableFindings(t, d) {
    const out = [], add = (level, text) => out.push({ level, text });
    const fs = d.fieldsOf(t.id), phiF = fs.filter(f => yes(f, "containsPHI")), piiF = fs.filter(f => yes(f, "containsPII"));
    if (!d.serverById.has(num(t, "sqlServerId"))) add("warn", `SQL server #${num(t, "sqlServerId")} doesn't exist`);
    if (!fs.length) add("info", "No columns recorded (use Import from SQL on the server)");
    else {
      if (!fs.some(f => yes(f, "isPrimaryKey"))) add("warn", "No primary key column");
      const unencPhi = phiF.filter(f => !yes(f, "isEncrypted")).length, unencPii = piiF.filter(f => !yes(f, "isEncrypted") && !yes(f, "containsPHI")).length;
      if (unencPhi) add(SC.unencryptedPhi, `${unencPhi} PHI column(s) not encrypted`);
      if (unencPii) add(SC.unencryptedPii, `${unencPii} PII column(s) not encrypted`);
      if (phiF.length && !yes(t, "containsPHI")) add("warn", `Has ${phiF.length} PHI column(s) but the table isn't flagged Contains PHI`);
      if (piiF.length && !yes(t, "containsPII")) add("warn", `Has ${piiF.length} PII column(s) but the table isn't flagged Contains PII`);
      if (yes(t, "containsPHI") && !phiF.length) add("info", "Flagged Contains PHI but no column is marked PHI");
      if (yes(t, "containsPII") && !piiF.length) add("info", "Flagged Contains PII but no column is marked PII");
      const looks = fs.filter(f => { const g = guess(str(f, "columnName")); return (g.phi && !yes(f, "containsPHI")) || (g.pii && !yes(f, "containsPII") && !yes(f, "containsPHI")); }).length;
      if (looks) add("info", `${looks} column name(s) look sensitive but aren't flagged`);
    }
    if ((yes(t, "containsPHI") || yes(t, "containsPII")) && !str(t, "description")) add("info", "Sensitive table has no description");
    const sens = yes(t, "containsPHI") || yes(t, "containsPII") || phiF.length || piiF.length;
    if (sens && !yes(t, "hasRowLevelSecurity") && !yes(t, "hasDynamicDataMasking") && fs.some(f => (yes(f, "containsPHI") || yes(f, "containsPII")) && !yes(f, "isEncrypted")))
      add("warn", "Unencrypted PHI / PII with neither row-level security nor dynamic data masking");
    if (yes(t, "hasDynamicDataMasking") && fs.length && !fs.some(f => str(f, "maskingRule"))) add("info", "Dynamic data masking is on but no column records a masking rule");
    const st = pick(t, "lastStatisticsUpdate"), days = st ? (Date.now() - (Cocky.parseDate(st)?.getTime() || Date.now())) / 864e5 : null;
    if (days != null && days > (SC.staleStatsDays || 90)) add("info", `Statistics last updated ${Math.round(days)} days ago`);
    if (sens && !/^(classified|approved)$/i.test(str(t, "classificationStatus"))) add("info", `Classification ${str(t, "classificationStatus") ? `is "${str(t, "classificationStatus")}"` : "not recorded"}`);
    return out;
  }
  // Version text for the end-of-support rules: MSSQL is written "SQL Server" there
  const versionText = s => { const ty = str(s, "databaseType"), v = str(s, "databaseVersion"); if (!v) return "";
    return `${/^ms ?sql$/i.test(ty) ? (/sql server/i.test(v) ? "" : "SQL Server ") : /postgre/i.test(ty) && !/postgre/i.test(v) ? "PostgreSQL " : /mysql/i.test(ty) && !/mysql/i.test(v) ? "MySQL " : ""}${v}`.trim(); };
  function serverFindings(s, d) {
    const out = [], add = (level, text) => out.push({ level, text });
    if (!SC.databaseTypes.some(x => lc(x) === lc(str(s, "databaseType")))) add("warn", `Database type "${str(s, "databaseType")}" isn't one of ${SC.databaseTypes.join(", ")}`);
    if (!str(s, "ownerName") && !str(s, "ownerEmail")) add("warn", "No owner recorded");
    if (!str(s, "dbaName") && !str(s, "dbaEmail")) add("warn", "No DBA recorded");
    if (!str(s, "databaseVersion")) add("info", "Version not recorded");
    if (!str(s, "environment")) add("info", "Environment not recorded");
    const holds = k => d.tablesOf(s.id).some(t => yes(t, k) || d.fieldsOf(t.id).some(f => yes(f, k)));
    const lower = l => l === "fail" ? "warn" : "info";
    if (!yes(s, "tdeEnabled")) {
      if (holds("containsPHI")) add(SC.noTdeWithPhi || "fail", "Holds PHI but TDE (encryption at rest) is off");
      else if (holds("containsPII")) add(lower(SC.noTdeWithPhi || "fail"), "Holds PII but TDE (encryption at rest) is off");
      else add("info", "TDE is off");
    }
    if (!yes(s, "defenderEnabled")) add(holds("containsPHI") || holds("containsPII") ? "warn" : "info", "Defender for SQL is off");
    if (!str(s, "backupPolicy")) add("warn", "No backup policy recorded");
    if (/prod/i.test(str(s, "environment")) && !yes(s, "geoReplicationEnabled")) add("info", "Production database without geo-replication");
    const vt = versionText(s);
    (CFG.infraAudit?.endOfSupport?.database || []).forEach(r => { try { if (vt && new RegExp(r.match, "i").test(vt)) add(r.level, `${r.text} (${vt})`); } catch { } });
    const ts = d.tablesOf(s.id);
    if (!ts.length) add("info", "No tables recorded (use Import from SQL)");
    // roll up the worst table problems
    const tf = ts.map(t => tableFindings(t, d)), bad = l => tf.filter(f => f.some(x => x.level === l)).length;
    if (bad("fail")) add("fail", `${bad("fail")} table(s) with failing findings (unencrypted PHI)`);
    else if (bad("warn")) add("warn", `${bad("warn")} table(s) with warnings`);
    return out;
  }
  const RANK = { fail: 3, warn: 2, info: 1 };
  function badge(f) {
    if (!f.length) return '<span class="badge text-bg-success">OK</span>';
    const worst = f.reduce((m, x) => Math.max(m, RANK[x.level] || 0), 0);
    const cls = worst === 3 ? "danger" : worst === 2 ? "warning" : "secondary", n = f.filter(x => x.level !== "info").length;
    return `<span class="badge text-bg-${cls}" title="${esc(f.slice().sort((a, b) => RANK[b.level] - RANK[a.level]).map(x => `${x.level.toUpperCase()}: ${x.text}`).join("\n"))}">${n ? `${n} issue${n === 1 ? "" : "s"}` : `${f.length} note${f.length === 1 ? "" : "s"}`}</span>`;
  }

  // ---------- Import from SQL ----------
  // SQL Server (incl. Azure SQL): every table and column with key, nullable, Always Encrypted and row-count details.
  // Run it in the catalogued database, then paste the result (SSMS: right-click the grid → Copy with Headers), CSV, or JSON.
  const IMPORT_QUERY = `SELECT s.name AS SchemaName, t.name AS TableName, c.name AS ColumnName, ty.name AS DataType,
  CASE WHEN ty.name IN ('nchar','nvarchar') AND c.max_length > 0 THEN c.max_length / 2 ELSE c.max_length END AS MaxLength,
  CASE WHEN pk.column_id IS NOT NULL THEN 1 ELSE 0 END AS IsPrimaryKey,
  CASE WHEN fk.parent_column_id IS NOT NULL THEN 1 ELSE 0 END AS IsForeignKey,
  CAST(c.is_nullable AS int) AS IsNullable,
  CASE WHEN c.encryption_type IS NOT NULL THEN 1 ELSE 0 END AS IsEncrypted,
  p.row_count AS SomeRowCount,
  CAST(sz.reserved_mb AS decimal(18,2)) AS TableSizeMB,
  st.last_update AS LastStatisticsUpdate,
  CASE WHEN rls.target_object_id IS NOT NULL THEN 1 ELSE 0 END AS HasRowLevelSecurity,
  CASE WHEN mt.object_id IS NOT NULL THEN 1 ELSE 0 END AS HasDynamicDataMasking,
  mc.masking_function AS MaskingRule
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.columns c ON c.object_id = t.object_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
LEFT JOIN (SELECT ic.object_id, ic.column_id FROM sys.indexes i
           JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
           WHERE i.is_primary_key = 1) pk ON pk.object_id = c.object_id AND pk.column_id = c.column_id
LEFT JOIN (SELECT DISTINCT parent_object_id, parent_column_id FROM sys.foreign_key_columns) fk
       ON fk.parent_object_id = c.object_id AND fk.parent_column_id = c.column_id
LEFT JOIN (SELECT object_id, SUM(rows) AS row_count FROM sys.partitions WHERE index_id IN (0, 1) GROUP BY object_id) p
       ON p.object_id = t.object_id
LEFT JOIN (SELECT object_id, SUM(reserved_page_count) * 8 / 1024.0 AS reserved_mb FROM sys.dm_db_partition_stats GROUP BY object_id) sz
       ON sz.object_id = t.object_id
OUTER APPLY (SELECT MAX(STATS_DATE(t.object_id, s2.stats_id)) AS last_update FROM sys.stats s2 WHERE s2.object_id = t.object_id) st
LEFT JOIN (SELECT DISTINCT target_object_id FROM sys.security_predicates) rls ON rls.target_object_id = t.object_id
LEFT JOIN (SELECT DISTINCT object_id FROM sys.masked_columns) mt ON mt.object_id = t.object_id
LEFT JOIN sys.masked_columns mc ON mc.object_id = c.object_id AND mc.column_id = c.column_id
WHERE t.is_ms_shipped = 0
ORDER BY s.name, t.name, c.column_id;`;

  // Parse pasted rows: JSON array, or tab / comma separated with a header row. Returns [{ ...by header }].
  function parsePaste(text) {
    text = String(text || "").trim();
    if (!text) return [];
    if (/^[\[{]/.test(text)) { const j = JSON.parse(text); return Array.isArray(j) ? j : [j]; }
    const lines = text.split(/\r?\n/).filter(l => l.trim());
    const delim = lines[0].includes("\t") ? "\t" : ",";
    const split = line => {
      if (delim === "\t") return line.split("\t");
      const out = []; let cur = "", q = false;
      for (let i = 0; i < line.length; i++) {
        const ch = line[i];
        if (q) { if (ch === '"' && line[i + 1] === '"') { cur += '"'; i++; } else if (ch === '"') q = false; else cur += ch; }
        else if (ch === '"') q = true; else if (ch === ",") { out.push(cur); cur = ""; } else cur += ch;
      }
      out.push(cur); return out;
    };
    const head = split(lines[0]).map(h => h.trim());
    return lines.slice(1).map(l => { const v = split(l); return Object.fromEntries(head.map((h, i) => [h, v[i] === undefined || v[i] === "NULL" ? null : v[i].trim()])); });
  }
  // Normalise pasted rows to [{ schemaName, tableName, columnName, dataType, maxLength, isPrimaryKey, isForeignKey, isNullable, isEncrypted, someRowCount }]
  function normalise(rows) {
    const b = v => v === true || v === 1 || /^(1|true|yes|y)$/i.test(String(v ?? "").trim());
    const n = v => v == null || v === "" || isNaN(+v) ? null : +v;
    return rows.map(r => ({
      schemaName: str(r, "schemaName") || str(r, "table_schema") || "dbo",
      tableName: str(r, "tableName") || str(r, "table_name"),
      columnName: str(r, "columnName") || str(r, "column_name"),
      dataType: str(r, "dataType") || str(r, "data_type") || "unknown",
      maxLength: n(pick(r, "maxLength") ?? pick(r, "character_maximum_length")),
      isPrimaryKey: b(pick(r, "isPrimaryKey")), isForeignKey: b(pick(r, "isForeignKey")),
      isNullable: pick(r, "isNullable") === undefined ? true : b(pick(r, "isNullable")),
      isEncrypted: b(pick(r, "isEncrypted")),
      someRowCount: n(pick(r, "someRowCount") ?? pick(r, "rowCount") ?? pick(r, "row_count")),
      tableSizeMB: n(pick(r, "tableSizeMB")),
      lastStatisticsUpdate: str(r, "lastStatisticsUpdate") ? (Cocky.parseDate(str(r, "lastStatisticsUpdate").replace(" ", "T"))?.toISOString() || null) : null,
      hasRowLevelSecurity: b(pick(r, "hasRowLevelSecurity")), hasDynamicDataMasking: b(pick(r, "hasDynamicDataMasking")),
      maskingRule: str(r, "maskingRule") || null,
      sensitivityLabel: str(r, "sensitivityLabel") || null
    })).filter(r => r.tableName && r.columnName);
  }
  // What an import would do on a server: { newTables: [{ schemaName, tableName, someRowCount, columns: [...] }], newColumns: [{ table, column }], existing: n }
  function plan(server, rows, d) {
    const key = (s, t) => `${lc(s)}.${lc(t)}`;
    const have = new Map(d.tablesOf(server.id).map(t => [key(str(t, "schemaName") || "dbo", str(t, "tableName")), t]));
    const byTable = new Map();
    rows.forEach(r => { const k = key(r.schemaName, r.tableName); if (!byTable.has(k)) byTable.set(k, { schemaName: r.schemaName, tableName: r.tableName, someRowCount: r.someRowCount,
      tableSizeMB: r.tableSizeMB, lastStatisticsUpdate: r.lastStatisticsUpdate, hasRowLevelSecurity: r.hasRowLevelSecurity, hasDynamicDataMasking: r.hasDynamicDataMasking, columns: [] }); byTable.get(k).columns.push(r); });
    const out = { newTables: [], newColumns: [], existing: 0, tables: byTable.size, refreshTables: [], refreshColumns: [] };
    const differs = (rec, facts) => Object.entries(facts).some(([k, v]) => { const cur = pick(rec, k);
      return k === "lastStatisticsUpdate" ? (Cocky.parseDate(cur)?.getTime() || 0) !== (Cocky.parseDate(v)?.getTime() || 0) : String(cur ?? "") !== String(v ?? ""); });
    byTable.forEach((t, k) => {
      const ex = have.get(k);
      if (!ex) { out.newTables.push(t); return; }
      // measured facts that changed on an existing table / column (Refresh option)
      const tf = { someRowCount: t.someRowCount, tableSizeMB: t.tableSizeMB, lastStatisticsUpdate: t.lastStatisticsUpdate, hasRowLevelSecurity: !!t.hasRowLevelSecurity, hasDynamicDataMasking: !!t.hasDynamicDataMasking };
      if (differs(ex, tf)) out.refreshTables.push({ table: ex, facts: tf });
      const cols = new Map(d.fieldsOf(ex.id).map(f => [lc(str(f, "columnName")), f]));
      t.columns.forEach(c => { const f = cols.get(lc(c.columnName));
        if (!f) { out.newColumns.push({ table: ex, column: c }); return; }
        out.existing++;
        const ff = { dataType: c.dataType, maxLength: c.maxLength, isPrimaryKey: c.isPrimaryKey, isForeignKey: c.isForeignKey, isNullable: c.isNullable, isEncrypted: c.isEncrypted, maskingRule: c.maskingRule || null };
        if (differs(f, ff)) out.refreshColumns.push({ field: f, facts: ff }); });
    });
    return out;
  }
  // Run a list of async jobs a few at a time; onProgress(done, total)
  async function runJobs(jobs, onProgress) {
    let next = 0, done = 0; const failures = [];
    const worker = async () => { while (next < jobs.length) { const j = jobs[next++]; try { await j(); } catch (e) { failures.push(e.message); } onProgress?.(++done, jobs.length); } };
    await Promise.all(Array.from({ length: Math.max(1, Math.min(SC.importBatch, jobs.length)) }, worker));
    return failures;
  }
  const fieldBody = (tableId, c, flags) => ({ sqlTableId: tableId, columnName: c.columnName.slice(0, 200), dataType: c.dataType.slice(0, 100), maxLength: c.maxLength,
    isPrimaryKey: c.isPrimaryKey, isForeignKey: c.isForeignKey, isNullable: c.isNullable, isEncrypted: c.isEncrypted,
    containsPHI: !!flags?.phi, containsPII: !!flags?.pii, description: null, sensitivityLabel: c.sensitivityLabel || null, maskingRule: c.maskingRule ? String(c.maskingRule).slice(0, 500) : null });

  window.SqlCatalog = { SC, pick, str, yes, num, loadAll, index, tableName, serverName, guess, fieldFindings, tableFindings, serverFindings, badge,
    IMPORT_QUERY, parsePaste, normalise, plan, runJobs, fieldBody, versionText };
})();
