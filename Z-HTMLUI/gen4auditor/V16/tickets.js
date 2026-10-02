// CockyAuditor trouble tickets (build 5.8): WorkerTroubleTickets, plus the problem classification and endpoint
// probe shared by Run Audit, Re-audit problems and Trouble Tickets. Load after app.js. Settings: config.js tickets.
(function () {
  const { api, esc, CFG, parseDate } = window.Cocky;
  const T = Object.assign({ numberPrefix: "CA", statuses: ["Open", "In Progress", "Resolved", "Closed"], closedStatuses: ["Resolved", "Closed"],
    severities: ["Critical", "High", "Medium", "Low"], problems: {}, defaultEnvironment: "Unknown" }, CFG.tickets || {});
  const PF = Object.assign({ warnMs: 2000, failMs: 5000 }, CFG.performance || {});
  const KINDS = Object.assign({
    down:  { label: "Hard down", severity: "Critical", ticketByDefault: true },
    error: { label: "Server error (5xx)", severity: "High", ticketByDefault: true },
    slow:  { label: "Very slow", severity: "Medium", ticketByDefault: false }
  }, T.problems);
  const KIND_BADGE = { down: "text-bg-danger", error: "text-bg-warning", slow: "text-bg-info", ok: "text-bg-success" };

  const val = v => (v == null || ["", "string"].includes(String(v).trim().toLowerCase())) ? "" : String(v).trim();
  const METHODS = ["GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS"];
  const methodOf = e => { const m = String(e.type || String(e.description || "").split(" ")[0] || "").toUpperCase(); return METHODS.includes(m) ? m : "GET"; };
  const joinUrl = (base, path) => { const b = String(base || "").replace(/\/+$/, ""), s = String(path || "").replace(/^\/+/, ""); return s ? `${b}/${s}` : b; };

  // Same rules as Run Audit: full url, else apiRoot + url, else host URL + apiRoot + url, else FQDN / host name / IP
  function targetUrl(e, hosts) {
    const u = val(e.url), root = val(e.apiRoot);
    if (/^https?:\/\//i.test(u)) return u;
    if (/^https?:\/\//i.test(root)) return joinUrl(root, u);
    const h = (hosts || []).find(x => x.id === e.apiHostId);
    if (h && val(h.apiHostUrl)) return joinUrl(joinUrl(val(h.apiHostUrl), root), u);
    const host = val(e.fqdn) || val(e.hostName) || val(e.iPv4Address);
    if (!host) return "";
    const scheme = e.sslSupported === false && e.httpSupported ? "http" : "https";
    const port = e.primaryPort > 0 && ![80, 443].includes(e.primaryPort) ? ":" + e.primaryPort : "";
    return joinUrl(joinUrl(`${scheme}://${host}${port}`, root), u);
  }

  // ---------- classification ----------
  // From a result { status, ms, reached }: "down" | "error" | "slow" | null (fine). status null + reached = CORS hid the code.
  function classify(r) {
    if (!r) return null;
    if (!r.reached && r.status == null) return r.tried === false ? null : "down";
    if (r.status === 0) return "down";
    if (r.status >= 500) return "error";
    if (r.ms != null && r.ms > PF.failMs) return "slow";
    return null;
  }
  // From the stamps Run Audit leaves on an endpoint (Apiaudit.LastStatusCode / LastResponseMs, build 5.6)
  function classifyStamp(e) {
    const s = e.lastStatusCode, ms = e.lastResponseMs;
    if (s == null && ms == null) return null;
    return classify({ status: s, ms, reached: s !== 0 && (s != null || ms != null) });
  }
  // From a Run Audit check (apiaudit.html auditOne: { url, status, ms, findings })
  function classifyCheck(c) {
    if (!c || c.running || !c.url) return null;
    const noReply = c.findings.some(f => f.level === "fail" && /No response within|Unreachable/.test(f.text));
    return classify({ status: c.status, ms: c.ms, reached: !noReply, tried: true });
  }
  const describe = (kind, r) => kind === "down" ? (r?.error || "No response")
    : kind === "error" ? `HTTP ${r?.status}` : kind === "slow" ? `${Math.round(r?.ms)} ms (limit ${PF.failMs} ms)` : "OK";
  const kindBadge = kind => kind ? `<span class="badge ${KIND_BADGE[kind] || "text-bg-secondary"}">${esc(KINDS[kind]?.label || kind)}</span>`
    : `<span class="badge text-bg-success">OK</span>`;

  // ---------- probe (Re-audit) ----------
  // One GET from this browser, like Run Audit: CORS first so the status can be read, then no-cors to see if it answers at all.
  async function probe(url, ms = CFG.auditTimeoutMs || 15000) {
    if (!url) return { url, status: null, ms: null, reached: false, tried: false, error: "No URL to test" };
    const go = async mode => {
      const ctrl = new AbortController(), t = setTimeout(() => ctrl.abort(), ms), start = performance.now();
      try { const res = await fetch(url, { method: "GET", mode, cache: "no-store", redirect: "follow", signal: ctrl.signal }); return { res, ms: performance.now() - start }; }
      finally { clearTimeout(t); }
    };
    const when = new Date().toISOString();
    try { const { res, ms: t } = await go("cors"); return { url, status: res.status, ms: t, reached: true, tried: true, when }; }
    catch (e) {
      if (e.name === "AbortError") return { url, status: null, ms: null, reached: false, tried: true, when, error: `No response within ${ms / 1000}s` };
      try { const { ms: t } = await go("no-cors"); return { url, status: null, ms: t, reached: true, tried: true, when, corsHidden: true }; }
      catch (e2) { return { url, status: null, ms: null, reached: false, tried: true, when,
        error: e2.name === "AbortError" ? `No response within ${ms / 1000}s` : "Unreachable (DNS, refused, TLS or blocked)" }; }
    }
  }
  // The stamps Run Audit writes (see apiaudit.html timing())
  const stampOf = r => ({ lastResponseMs: r.ms != null ? Math.max(0, Math.round(r.ms)) : null,
    lastStatusCode: r.status != null ? r.status : (!r.reached && r.tried ? 0 : null) });

  // ---------- tickets ----------
  const isOpen = t => !T.closedStatuses.map(s => s.toLowerCase()).includes(String(t.status || "").toLowerCase());
  const norm = s => String(s || "").trim().toLowerCase();
  // What a ticket records for an endpoint: ApiName = host (API) name, Endpoint = "GET /api/x" (the URL tested underneath)
  function ticketKey(e, hosts) {
    const h = (hosts || []).find(x => x.id === e.apiHostId);
    const apiName = val(h?.apiHostName) || val(e.applicationName) || val(e.family) || "Unknown API";
    const path = val(e.url) || val(e.description) || "#" + e.id;
    return { apiName, endpoint: `${methodOf(e)} ${path}`.slice(0, 500) };
  }
  const openTicketFor = (tickets, e, hosts) => {
    const k = ticketKey(e, hosts);
    return (tickets || []).filter(isOpen).find(t => norm(t.apiName) === norm(k.apiName) && norm(t.endpoint) === norm(k.endpoint)) || null;
  };
  const stamp = d => d.toISOString().replace(/[-:]/g, "").replace("T", "-").slice(0, 15);
  const who = () => { const u = window.CockyAuth?.current?.() || {}; return u.fullname || u.username || u.email || "CockyAuditor"; };

  // Ownership for a ticket: the applications linked to the endpoint's host (ApplicationApi), else the host / endpoint's own fields.
  // ctx: { apps, links }
  function ownerOf(e, hosts, ctx = {}) {
    const h = (hosts || []).find(x => x.id === e.apiHostId);
    const appIds = new Set((ctx.links || []).filter(l => l.apiHostId === e.apiHostId).map(l => l.applicationId));
    const apps = (ctx.apps || []).filter(a => appIds.has(a.id));
    const uniq = xs => [...new Set(xs.map(val).filter(Boolean))].join(", ");
    return {
      applicationOwner: (uniq(apps.map(a => a.ownerName)) || val(e.techContactName) || val(h?.techContactName)).slice(0, 200) || null,
      businessUnit: (uniq(apps.map(a => a.businessUnit)) || val(e.businessUnitName) || val(h?.businessUnitName)).slice(0, 100) || null,
      apps: apps.map(a => val(a.applicationName)).filter(Boolean)
    };
  }
  // The check's own findings, for AuditorNotes (Run Audit passes them in r.findings)
  const findingsText = r => (r?.findings || []).filter(f => f.level !== "pass").map(f => `${f.level.toUpperCase()}: ${f.text}`).join("\n");

  // New ticket body for an endpoint problem. r = the result ({ status, ms, url, error, findings? }). ctx: { apps, links }
  function newTicket(e, hosts, kind, r, source, ctx) {
    const h = (hosts || []).find(x => x.id === e.apiHostId), k = ticketKey(e, hosts), K = KINDS[kind] || {}, o = ownerOf(e, hosts, ctx);
    return {
      ticketNumber: `${T.numberPrefix}-${stamp(new Date())}-${e.id}`,
      apiName: k.apiName.slice(0, 200), endpoint: k.endpoint,
      environment: (val(e.environment) || val(h?.environment) || T.defaultEnvironment).slice(0, 50),
      severity: K.severity || "High", status: T.statuses[0] || "Open", reportedBy: who().slice(0, 200),
      description: [`${K.label || kind}: ${describe(kind, r)}`, r?.url ? `URL: ${r.url}` : "", `Found by ${source} on ${new Date().toLocaleString()}.`, `Endpoint #${e.id}.`]
        .filter(Boolean).join("\n").slice(0, 4000),
      isHardDown: kind === "down", incidentCount: 1, assignedTo: null, resolutionNotes: null, resolvedOn: null,
      // Build 5.8 additions: filled where the auditor knows them; ImpactedUsers and RootCause are left for people
      impactedUsers: null, rootCause: null,
      businessUnit: o.businessUnit, applicationOwner: o.applicationOwner,
      evidenceUrl: r?.url ? String(r.url).slice(0, 1000) : null,
      auditorNotes: ([`${source}, ${new Date().toLocaleString()}: ${K.label || kind}, ${describe(kind, r)}.`, o.apps.length ? `Applications: ${o.apps.join(", ")}.` : "", findingsText(r)]
        .filter(Boolean).join("\n")).slice(0, 4000)
    };
  }
  // Raise IncidentCount on an open ticket; a hard-down recurrence also escalates IsHardDown / severity
  function recurrence(t, kind, r, source) {
    const K = KINDS[kind] || {}, rank = s => { const i = T.severities.findIndex(x => norm(x) === norm(s)); return i < 0 ? 99 : i; };
    return { ...t, incidentCount: (t.incidentCount || 1) + 1, isHardDown: t.isHardDown || kind === "down",
      severity: rank(K.severity) < rank(t.severity) ? K.severity : t.severity,
      description: `${t.description || ""}\nAgain ${new Date().toLocaleString()} (${source}): ${K.label || kind}, ${describe(kind, r)}.`.slice(-4000),
      auditorNotes: `${t.auditorNotes || ""}\n${source}, ${new Date().toLocaleString()}: ${K.label || kind}, ${describe(kind, r)}.`.trim().slice(-4000),
      evidenceUrl: t.evidenceUrl || (r?.url ? String(r.url).slice(0, 1000) : null) };
  }
  // items: [{ endpoint, kind, result }]. Creates a ticket per endpoint, or raises the incident count on its open ticket.
  // Returns { created: [...], updated: [...], failures: [msg] }.
  async function fileTickets(items, hosts, source, tickets) {
    const all = tickets || await api.list("troubletickets");
    // Applications linked to the hosts, for Application owner / Business unit (missing routes just leave them blank)
    const [apps, links] = await Promise.all([api.list("apps").catch(() => []), api.list("apphosts").catch(() => [])]);
    const ctx = { apps, links };
    const out = { created: [], updated: [], failures: [] };
    for (const it of items) {
      const existing = openTicketFor(all, it.endpoint, hosts);
      try {
        if (existing) {
          const body = recurrence(existing, it.kind, it.result, source);
          await api.update("troubletickets", existing.id, body); Object.assign(existing, body); out.updated.push(existing);
        } else {
          const body = newTicket(it.endpoint, hosts, it.kind, it.result, source, ctx);
          const saved = await api.create("troubletickets", body);
          const t = saved && typeof saved === "object" ? saved : body; all.push(t); out.created.push(t);
        }
      } catch (e) { out.failures.push(`#${it.endpoint.id}: ${e.message}`); }
    }
    return out;
  }
  async function resolve(t, notes, rootCause) {
    const body = { ...t, status: "Resolved", resolvedOn: new Date().toISOString(), resolutionNotes: notes || t.resolutionNotes || null };
    if (rootCause) body.rootCause = rootCause.slice(0, 2000);
    await api.update("troubletickets", t.id, body);
    return Object.assign(t, body);
  }

  // A dialog that lists what will happen and asks to go ahead. rows: [html]. Resolves true / false.
  function confirmList(title, intro, rows, okText = "Go ahead") {
    return new Promise(res => {
      document.getElementById("tkConfirm")?.remove();
      document.body.insertAdjacentHTML("beforeend", `<div class="modal fade" id="tkConfirm" tabindex="-1"><div class="modal-dialog modal-lg modal-dialog-scrollable"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">${esc(title)}</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
        <div class="modal-body"><p class="small">${intro}</p><ul class="small mb-0">${rows.map(r => `<li>${r}</li>`).join("")}</ul></div>
        <div class="modal-footer"><button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Cancel</button>
          <button class="btn btn-primary ok" type="button">${esc(okText)}</button></div></div></div></div>`);
      const el = document.getElementById("tkConfirm"), m = bootstrap.Modal.getOrCreateInstance(el);
      let answer = false;
      el.querySelector(".ok").onclick = () => { answer = true; m.hide(); };
      el.addEventListener("hidden.bs.modal", () => { el.remove(); res(answer); });
      m.show();
    });
  }
  // Describe what fileTickets would do, for confirmList
  function planRows(items, hosts, tickets) {
    return items.map(it => {
      const k = ticketKey(it.endpoint, hosts), t = openTicketFor(tickets, it.endpoint, hosts);
      return `${kindBadge(it.kind)} <b>${esc(k.apiName)}</b> ${esc(k.endpoint)} · ${esc(describe(it.kind, it.result))} → ` +
        (t ? `raise incidents on <b>${esc(t.ticketNumber || "#" + t.id)}</b> to ${(t.incidentCount || 1) + 1}` : `new <b>${esc(KINDS[it.kind]?.severity || "")}</b> ticket`);
    });
  }

  window.Tickets = { T, KINDS, PF, val, methodOf, targetUrl, classify, classifyStamp, classifyCheck, describe, kindBadge, probe, stampOf,
    isOpen, ticketKey, openTicketFor, ownerOf, newTicket, recurrence, fileTickets, resolve, confirmList, planRows, parseDate };
})();
