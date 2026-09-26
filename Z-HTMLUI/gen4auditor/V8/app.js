// CockyAuditor shared code: layout, API client, tables, add/edit dialog.
(function () {
  const CFG = window.COCKY_CONFIG;
  const SCHEMA = window.COCKY_SCHEMA;

  const PAGES = [
    { key: "dashboard",  href: "apidashboard.html",  label: "Dashboard" },
    { key: "apps",       href: "apiapps.html",       label: "Apps" },
    { key: "hosts",      href: "apihosts.html",      label: "Hosts" },
    { key: "endpoints",  href: "apiendpoints.html",  label: "Endpoints" },
    { key: "interfaces", href: "apiinterfaces.html", label: "Interfaces" },
    { key: "audit",      href: "apiaudit.html",      label: "Run Audit" },
    { key: "exceptions", href: "apiexceptions.html", label: "Exceptions" },
    { key: "history",    href: "apihistory.html",    label: "History" },
    { key: "logs",       href: "enterprise9.html",   label: "Enterprise(9)" }
  ];

  // ---------- helpers ----------
  const esc = s => String(s ?? "").replace(/[&<>"']/g, c =>
    ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));

  const titleCase = k => String(k)
    .replace(/([a-z0-9])([A-Z])/g, "$1 $2")
    .replace(/^./, c => c.toUpperCase());

  const DATE_RE = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}/;

  // The API returns UTC dates without a zone ("2026-09-22T21:10:35.369").
  // Treat those as UTC instead of the browser's local time.
  function parseDate(v) {
    if (v == null || v === "") return null;
    if (typeof v === "string" && DATE_RE.test(v) && !/(Z|[+-]\d{2}:?\d{2})$/i.test(v)) v += "Z";
    const d = new Date(v);
    return isNaN(d) ? null : d;
  }
  const dateMs = v => parseDate(v)?.getTime() ?? 0;

  function fieldType(resource, key, sample) {
    const t = SCHEMA[resource]?.fields?.[key];
    if (t) return t;
    if (typeof sample === "boolean") return "bool";
    if (typeof sample === "number") return "number";
    if (typeof sample === "string" && DATE_RE.test(sample)) return "date";
    if (/date$/i.test(key)) return "date";
    if (/^(is|has)[A-Z]/.test(key)) return "bool";
    return "string";
  }

  function fmtValue(v, type) {
    if (type === "password") return v ? "••••••" : '<span class="text-muted">—</span>';
    if (refTarget(type) && v != null && v !== "") return esc(refText(type, v));
    if (v === null || v === undefined || v === "") return '<span class="text-muted">—</span>';
    if (type === "bool" || typeof v === "boolean")
      return v ? '<span class="badge text-bg-success">Yes</span>' : '<span class="badge text-bg-secondary">No</span>';
    if (type === "date" || (typeof v === "string" && DATE_RE.test(v))) {
      const d = parseDate(v);
      if (d) return esc(d.toLocaleString());
    }
    const s = typeof v === "object" ? JSON.stringify(v) : String(v);
    return s.length > 60 ? `<span title="${esc(s)}">${esc(s.slice(0, 57))}…</span>` : esc(s);
  }

  // ---------- lookups for "ref:<resource>" fields ----------
  const refCache = {};
  const refLabel = r => `#${r.id} ${r.applicationName || r.description || r.apiHostName || r.name || ""}`.trim();
  const refTarget = type => typeof type === "string" && type.startsWith("ref:") ? type.slice(4) : null;
  async function ensureRefs(resource) {
    const targets = [...new Set(Object.values(SCHEMA[resource]?.fields || {}).map(refTarget).filter(Boolean))];
    await Promise.all(targets.filter(t => !refCache[t]).map(t =>
      api.list(t).then(rows => { refCache[t] = rows; }).catch(() => { refCache[t] = null; })));
  }
  function refText(type, v) {
    const rows = refCache[refTarget(type)];
    const hit = rows && rows.find(r => String(r.id) === String(v));
    return hit ? refLabel(hit) : `#${v}`;
  }

  // Value used when the API requires a field the user left blank
  function fillerFor(type, current) {
    switch (type) {
      case "number": return 0;
      case "bool": return false;
      case "date": return new Date().toISOString();
      case "guid": return "00000000-0000-0000-0000-000000000000";
      default: return typeof current === "number" ? 0 : "N/A";
    }
  }

  // ---------- API client ----------
  function url(resource, suffix = "") {
    const path = CFG.endpoints[resource];
    if (!path) throw new Error(`Unknown resource "${resource}" - check config.js`);
    return CFG.apiBaseUrl.replace(/\/$/, "") + path + suffix;
  }

  async function request(method, resource, suffix = "", body) {
    const target = url(resource, suffix);
    const ctrl = new AbortController();
    const timer = setTimeout(() => ctrl.abort(), CFG.requestTimeoutMs);
    let res;
    try {
      res = await fetch(target, {
        method,
        headers: body !== undefined ? { "Content-Type": "application/json", Accept: "application/json" } : { Accept: "application/json" },
        body: body !== undefined ? JSON.stringify(body) : undefined,
        signal: ctrl.signal
      });
    } catch (e) {
      const err = new Error(e.name === "AbortError"
        ? `Timed out after ${CFG.requestTimeoutMs / 1000}s waiting for ${method} ${target}. The API may be hanging on this route.`
        : `Could not reach ${method} ${target}. This is usually CORS (the API must allow this page's origin: ${location.origin}) or the API being down.`);
      err.kind = e.name === "AbortError" ? "timeout" : "network";
      throw err;
    } finally {
      clearTimeout(timer);
    }
    const text = await res.text();
    let data = null;
    if (text) { try { data = JSON.parse(text); } catch { data = text; } }
    if (!res.ok) {
      let detail = "";
      if (data && typeof data === "object") {
        detail = data.title || data.message || "";
        if (data.errors) detail += " " + Object.entries(data.errors).map(([k, v]) => `${k}: ${[].concat(v).join(", ")}`).join("; ");
      } else if (typeof data === "string") detail = data.slice(0, 300);
      const err = new Error(`${method} ${target} returned ${res.status} ${res.statusText}${detail ? " - " + detail : ""}`);
      err.status = res.status; err.kind = "http";
      if (data && typeof data === "object" && data.errors) err.fields = Object.keys(data.errors).filter(k => k && k !== "$" && !/^(body|request)$/i.test(k));
      throw err;
    }
    return data;
  }

  const api = {
    list: (r, suffix = "") => request("GET", r, suffix).then(d => Array.isArray(d) ? d : (d == null ? [] : [d])),
    get: (r, id) => request("GET", r, "/" + encodeURIComponent(id)),
    // ids are generated by SQL Server, so never send one on create
    create: (r, body) => { const b = { ...body }; delete b.id; delete b.Id; return request("POST", r, "", b); },
    update: (r, id, body) => request("PUT", r, "/" + encodeURIComponent(id), body),
    remove: (r, id) => request("DELETE", r, "/" + encodeURIComponent(id)),
    // POST, and if the API answers 400 naming fields it requires (non-nullable string/Guid/number/date in the C# model),
    // fill just those with a harmless value and try again (up to 3 rounds, since .NET may report one bad field at a time).
    // Resolves { data, filled: [field names] }.
    async createFilling(r, body) {
      let current = { ...body };
      const filled = [];
      for (let round = 0; ; round++) {
        try { return { data: await api.create(r, current), filled }; }
        catch (e) {
          if (e.status !== 400 || !e.fields?.length || round >= 3) throw e;
          let changed = false;
          for (const raw of e.fields) {
            const k = raw.replace(/^\$\./, "").split(/[.\[]/)[0];
            const key = Object.keys(current).find(x => x.toLowerCase() === k.toLowerCase()) ||
                        Object.keys(SCHEMA[r]?.fields || {}).find(x => x.toLowerCase() === k.toLowerCase());
            if (!key || filled.includes(key)) continue;
            current[key] = fillerFor(SCHEMA[r]?.fields?.[key], current[key]);
            filled.push(key); changed = true;
          }
          if (!changed) throw e;
        }
      }
    },
    url
  };

  // ---------- layout ----------
  function layout(activeKey) {
    const page = PAGES.find(p => p.key === activeKey);
    document.title = `${page ? page.label + " · " : ""}${CFG.appName}`;
    const nav = PAGES.map(p =>
      `<a href="${p.href}" class="${p.key === activeKey ? "active" : ""}">${esc(p.label)}</a>`).join("");
    document.body.insertAdjacentHTML("afterbegin", `
      <div class="app">
        <nav class="sidebar">
          <a class="brand" href="index.html" title="Home">${esc(CFG.appName)}</a>
          ${nav}
          <div class="user-box">${esc(window.CockyAuth?.current().username || "")}<a href="#" id="signOut">Sign out</a></div>
          <div class="api-note" title="${esc(CFG.apiBaseUrl)}">Build ${esc(CFG.version || "")}${CFG.builds?.[0] ? " · " + esc(CFG.builds[0].title) : ""}<br>API: ${esc(new URL(CFG.apiBaseUrl).host.split(".")[0])}${CFG.copyright ? `<br><span class="copy">${esc(CFG.copyright)}</span>` : ""}</div>
        </nav>
        <main class="content" id="page"></main>
      </div>
      <div class="toast-container position-fixed bottom-0 end-0 p-3" id="toasts"></div>`);
    document.getElementById("signOut").addEventListener("click", e => { e.preventDefault(); window.CockyAuth?.signOut(); });
    const main = document.getElementById("page");
    // Move page content (anything already in <body> marked data-page) into main
    document.querySelectorAll("[data-page]").forEach(el => main.appendChild(el));
    return main;
  }

  function toast(message, type = "success") {
    const el = document.createElement("div");
    el.className = `toast align-items-center text-bg-${type} border-0`;
    el.innerHTML = `<div class="d-flex"><div class="toast-body">${esc(message)}</div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button></div>`;
    document.getElementById("toasts").appendChild(el);
    const t = new bootstrap.Toast(el, { delay: type === "danger" ? 8000 : 3000 });
    el.addEventListener("hidden.bs.toast", () => el.remove());
    t.show();
  }

  function errorBox(err, retry) {
    const div = document.createElement("div");
    div.className = "alert alert-danger d-flex justify-content-between align-items-start gap-3";
    div.innerHTML = `<div><strong>Couldn't load data.</strong><div class="small mt-1">${esc(err.message)}</div></div>`;
    if (retry) {
      const b = document.createElement("button");
      b.className = "btn btn-sm btn-outline-danger"; b.textContent = "Retry"; b.onclick = retry;
      div.appendChild(b);
    }
    return div;
  }

  const spinner = (text = "Loading…") =>
    `<div class="text-muted py-4"><span class="spinner-border spinner-border-sm me-2"></span>${esc(text)}</div>`;

  // ---------- table ----------
  function columnsFor(resource, rows) {
    const preferred = SCHEMA[resource]?.columns || [];
    if (!rows.length) return preferred;
    const keys = new Set(rows.flatMap(r => Object.keys(r)));
    const cols = preferred.filter(c => keys.has(c));
    if (cols.length >= 3) return cols;
    return [...keys].slice(0, 8);
  }

  function renderTable(container, resource, rows, opts = {}) {
    const cols = opts.columns || columnsFor(resource, rows);
    const label = SCHEMA[resource]?.labelPlural || resource;
    if (!rows.length) {
      container.innerHTML = `<div class="empty">${esc(opts.emptyText || `No ${label.toLowerCase()} yet.`)}</div>`;
      return;
    }
    const actions = opts.onEdit || opts.onDelete || opts.rowActions;
    container.innerHTML = `
      <div class="table-responsive"><table class="table table-hover table-sm align-middle mb-0">
        <thead><tr>${cols.map(c => `<th>${esc(SCHEMA[resource]?.titles?.[c] || titleCase(c))}</th>`).join("")}${actions ? "<th></th>" : ""}</tr></thead>
        <tbody>${rows.map((r, i) => `<tr data-i="${i}" class="${opts.onRowClick ? "clickable" : ""}">
          ${cols.map(c => `<td>${SCHEMA[resource]?.format?.[c] ? SCHEMA[resource].format[c](r[c], r) : fmtValue(r[c], fieldType(resource, c, r[c]))}</td>`).join("")}
          ${actions ? `<td class="text-end text-nowrap">
            ${(opts.rowActions || []).map((a, ai) => { const lbl = typeof a.label === "function" ? a.label(r) : esc(a.label);
              const tip = typeof a.title === "function" ? a.title(r) : a.title;
              return `<button class="btn btn-sm ${a.className || "btn-outline-primary"} me-1" data-act="x${ai}"${tip ? ` title="${esc(tip)}"` : ""}>${lbl}</button>`; }).join("")}
            ${opts.onEdit ? '<button class="btn btn-sm btn-outline-secondary me-1" data-act="edit">Edit</button>' : ""}
            ${opts.onDelete ? '<button class="btn btn-sm btn-outline-danger" data-act="del">Delete</button>' : ""}
          </td>` : ""}
        </tr>`).join("")}</tbody>
      </table></div>`;
    container.querySelectorAll("tbody tr").forEach(tr => {
      const row = rows[+tr.dataset.i];
      tr.addEventListener("click", e => {
        const act = e.target.closest("[data-act]")?.dataset.act;
        if (act === "edit") return opts.onEdit(row);
        if (act === "del") return opts.onDelete(row);
        if (act?.startsWith("x")) return opts.rowActions[+act.slice(1)].onClick(row);
        if (opts.onRowClick) opts.onRowClick(row);
      });
    });
  }

  function filterRows(rows, q) {
    q = q.trim().toLowerCase();
    if (!q) return rows;
    return rows.filter(r => Object.values(r).some(v => v != null && String(v).toLowerCase().includes(q)));
  }

  // ---------- add/edit dialog ----------
  let modalEl;
  function ensureModal() {
    if (modalEl) return modalEl;
    document.body.insertAdjacentHTML("beforeend", `
      <div class="modal fade" id="recordModal" tabindex="-1"><div class="modal-dialog modal-lg modal-dialog-scrollable"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title"></h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
        <div class="modal-body">
          <div class="guess-note alert alert-warning small d-none"></div>
          <ul class="nav nav-tabs mb-3">
            <li class="nav-item"><button class="nav-link active" data-tab="form" type="button">Form</button></li>
            <li class="nav-item"><button class="nav-link" data-tab="json" type="button">JSON</button></li>
          </ul>
          <form class="tab-form row g-3"></form>
          <div class="tab-json d-none">
            <textarea class="form-control font-monospace" rows="18" spellcheck="false"></textarea>
            <div class="form-text">Edit the exact JSON that will be sent. Useful if the API expects fields the form doesn't show.</div>
          </div>
          <div class="save-error alert alert-danger small mt-3 d-none"></div>
        </div>
        <div class="modal-footer"><button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Cancel</button>
          <button class="btn btn-primary save-btn" type="button">Save</button></div>
      </div></div></div>`);
    modalEl = document.getElementById("recordModal");
    return modalEl;
  }

  function toLocalInput(v) {
    if (!v) return "";
    const d = parseDate(v);
    if (!d) return "";
    const p = n => String(n).padStart(2, "0");
    return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}T${p(d.getHours())}:${p(d.getMinutes())}`;
  }

  function inputFor(key, type, value, isNew) {
    const id = "f_" + key;
    const lbl = `<label class="form-label small mb-1" for="${id}">${esc(titleCase(key))}</label>`;
    const ro = key === "id" ? "readonly" : "";
    if (key === "id" && isNew) return "";
    switch (type) {
      case "bool":
        return `<div class="col-md-4 d-flex align-items-end"><div class="form-check">
          <input class="form-check-input" type="checkbox" id="${id}" data-key="${key}" data-type="bool" ${value ? "checked" : ""}>
          <label class="form-check-label" for="${id}">${esc(titleCase(key))}</label></div></div>`;
      case "number":
        return `<div class="col-md-4">${lbl}<input class="form-control form-control-sm" type="number" id="${id}" data-key="${key}" data-type="number" value="${value ?? ""}" ${ro}></div>`;
      case "date":
        return `<div class="col-md-4">${lbl}<input class="form-control form-control-sm" type="datetime-local" id="${id}" data-key="${key}" data-type="date" value="${toLocalInput(value)}"></div>`;
      case "password":
        return `<div class="col-md-6">${lbl}<div class="input-group input-group-sm">
          <input class="form-control" type="password" autocomplete="new-password" id="${id}" data-key="${key}" data-type="password" value="${esc(value ?? "")}">
          <button class="btn btn-outline-secondary" type="button" onclick="const i=this.previousElementSibling;i.type=i.type==='password'?'text':'password'">Show</button></div></div>`;
      case (refTarget(type) ? type : "\u0000"): {
        const rows = refCache[refTarget(type)] || [];
        const known = rows.some(r => String(r.id) === String(value));
        const opts = ['<option value="">(none)</option>']
          .concat(rows.map(r => `<option value="${r.id}" ${String(r.id) === String(value) ? "selected" : ""}>${esc(refLabel(r))}</option>`))
          .concat(value != null && value !== "" && !known ? [`<option value="${esc(value)}" selected>#${esc(value)} (not found)</option>`] : []);
        return `<div class="col-md-6">${lbl}<select class="form-select form-select-sm" id="${id}" data-key="${key}" data-type="ref">${opts.join("")}</select></div>`;
      }
      case "text":
        return `<div class="col-12">${lbl}<textarea class="form-control form-control-sm" rows="2" id="${id}" data-key="${key}" data-type="text">${esc(value ?? "")}</textarea></div>`;
      default:
        return `<div class="col-md-6">${lbl}<input class="form-control form-control-sm" type="text" id="${id}" data-key="${key}" data-type="${type}" value="${esc(value ?? "")}" ${ro}></div>`;
    }
  }

  function readForm(form, base) {
    const out = { ...base };
    form.querySelectorAll("[data-key]").forEach(el => {
      const k = el.dataset.key, t = el.dataset.type;
      if (t === "bool") out[k] = el.checked;
      else if (t === "number" || t === "ref") out[k] = el.value === "" ? null : Number(el.value);
      else if (t === "date") out[k] = el.value ? new Date(el.value).toISOString() : null;
      else out[k] = el.value === "" ? null : el.value;
    });
    return out;
  }

  // Opens the add/edit dialog. `sampleRows` = existing records, used to learn field names.
  // defaults: starting values for a new record (e.g. { apiHostId: 3 })
  // opts.view: "core" (groups without detail:true), "details" (only detail groups), or "all". Fields not shown
  // are kept unchanged from the record when saving.
  async function openRecordForm(resource, record, sampleRows = [], defaults = {}, opts = {}) {
    await ensureRefs(resource);
    return new Promise(resolve => {
      const m = ensureModal();
      const isNew = !record;
      const sch = SCHEMA[resource] || { fields: {} };
      const learned = sampleRows.length ? Object.keys(Object.assign({}, ...sampleRows)) : [];
      let keys = [...new Set([...(record ? Object.keys(record) : learned.length ? learned : Object.keys(sch.fields))])];
      const view = opts.view || sch.defaultView || "all";
      let schGroups = sch.groups;
      if (schGroups && view !== "all") {
        const detailKeys = new Set(schGroups.filter(g => g.detail).flatMap(g => g.keys));
        schGroups = schGroups.filter(g => view === "details" ? g.detail : !g.detail);
        keys = keys.filter(k => view === "details" ? detailKeys.has(k) : !detailKeys.has(k));
      }
      const sample = record || Object.assign({}, ...sampleRows);
      const base = {};
      if (isNew) keys.forEach(k => { if (k !== "id") base[k] = fieldType(resource, k, sample[k]) === "bool" ? /^(is)?active$/i.test(k) : null; });
      if (isNew) Object.entries(defaults).forEach(([k, v]) => { base[k] = v; if (!keys.includes(k)) keys.push(k); });

      m.querySelector(".modal-title").textContent = view === "details" && record
        ? `${sch.label || resource} details: ${record.apiHostName || record.description || "#" + record.id}`
        : `${isNew ? "Add" : "Edit"} ${sch.label || resource}${record ? " #" + record.id : ""}`;
      const note = m.querySelector(".guess-note");
      const guessing = !sch.verified && !learned.length && !record;
      note.classList.toggle("d-none", !guessing);
      note.textContent = guessing
        ? `There are no ${(sch.labelPlural || resource).toLowerCase()} yet, so these fields are a best guess. Compare with the Swagger schema; if the save fails, adjust in the JSON tab (and in schema.js).`
        : "";
      const form = m.querySelector(".tab-form");
      const field = k => inputFor(k, fieldType(resource, k, sample[k]), record ? record[k] : base[k], isNew);
      if (schGroups) {
        // Grouped sections; anything not in a group goes under "Other"
        const placed = new Set(sch.groups.flatMap(g => g.keys));
        const groups = schGroups.map(g => ({ ...g, keys: g.keys.filter(k => keys.includes(k)) }))
          .concat([{ title: "Other", keys: keys.filter(k => !placed.has(k)) }]).filter(g => g.keys.length);
        const filled = g => record && g.keys.some(k => k !== "id" && record[k] !== null && record[k] !== "" && record[k] !== false && record[k] !== 0 && record[k] !== "string");
        form.innerHTML = groups.map((g, i) => `<details class="col-12 form-section" ${i === 0 || filled(g) ? "open" : ""}>
          <summary>${esc(g.title)} <span class="text-muted small fw-normal">(${g.keys.length})</span></summary>
          <div class="row g-3 pt-2">${g.keys.map(field).join("")}</div></details>`).join("");
      } else form.innerHTML = keys.map(field).join("");
      const ta = m.querySelector(".tab-json textarea");
      const errBox = m.querySelector(".save-error");
      errBox.classList.add("d-none");
      let tab = "form";
      const current = () => tab === "form" ? readForm(form, record || base) : JSON.parse(ta.value);

      m.querySelectorAll("[data-tab]").forEach(btn => btn.onclick = () => {
        try {
          if (btn.dataset.tab === "json" && tab === "form") ta.value = JSON.stringify(readForm(form, record || base), null, 2);
          if (btn.dataset.tab === "form" && tab === "json") {
            const obj = JSON.parse(ta.value);
            form.querySelectorAll("[data-key]").forEach(el => {
              const v = obj[el.dataset.key];
              if (el.dataset.type === "bool") el.checked = !!v;
              else if (el.dataset.type === "date") el.value = toLocalInput(v);
              else el.value = v ?? "";
            });
          }
        } catch (e) { errBox.textContent = "JSON is not valid: " + e.message; errBox.classList.remove("d-none"); return; }
        errBox.classList.add("d-none");
        tab = btn.dataset.tab;
        m.querySelectorAll("[data-tab]").forEach(b => b.classList.toggle("active", b === btn));
        form.classList.toggle("d-none", tab !== "form");
        m.querySelector(".tab-json").classList.toggle("d-none", tab !== "json");
      });
      m.querySelector('[data-tab="form"]').click();

      const saveBtn = m.querySelector(".save-btn");
      saveBtn.onclick = async () => {
        let body;
        try { body = current(); } catch (e) { errBox.textContent = "JSON is not valid: " + e.message; errBox.classList.remove("d-none"); return; }
        if (!isNew) { body.id = record.id; }
        if (isNew) { const now = new Date().toISOString(); ["createdDate", "modifiedDate"].forEach(k => { if (k in body && !body[k]) body[k] = now; }); }
        else if ("modifiedDate" in body) body.modifiedDate = new Date().toISOString();
        saveBtn.disabled = true; saveBtn.textContent = "Saving…";
        try {
          let saved, filledKeys = [];
          if (isNew) ({ data: saved, filled: filledKeys } = await api.createFilling(resource, body));
          else saved = await api.update(resource, record.id, body);
          bs.hide(); saved_ = saved ?? body;
          toast(`${sch.label || "Record"} ${isNew ? "added" : "saved"}.` +
            (filledKeys.length ? ` The API required ${filledKeys.length} blank field(s), filled with N/A/0: ${filledKeys.join(", ")}.` : ""),
            filledKeys.length ? "warning" : "success");
        } catch (e) {
          errBox.textContent = e.message; errBox.classList.remove("d-none");
        } finally { saveBtn.disabled = false; saveBtn.textContent = "Save"; }
      };
      let saved_ = null;
      const bs = bootstrap.Modal.getOrCreateInstance(m);
      m.addEventListener("hidden.bs.modal", () => resolve(saved_), { once: true });
      bs.show();
    });
  }

  async function confirmDelete(resource, row, beforeDelete) {
    const sch = SCHEMA[resource] || {};
    const name = row.applicationName || row.description || row.apiHostName || row.hostName || row.name || "";
    if (!confirm(`Delete ${sch.label || "record"} #${row.id}${name ? ` (${name})` : ""}? This can't be undone.`)) return false;
    let undo = null;
    try {
      undo = await beforeDelete?.(row);   // may return a function that reverses its changes
      await api.remove(resource, row.id);
      toast(`${sch.label || "Record"} #${row.id} deleted.`);
      return true;
    } catch (e) {
      if (typeof undo === "function") { try { await undo(); } catch (u) { e.message += ` (and restoring the removed links failed: ${u.message})`; } }
      toast(e.message, "danger"); return false;
    }
  }

  // ---------- full CRUD page ----------
  // Used by Apps / Hosts / Endpoints / Exceptions. `source()` returns the list suffix to load.
  // columns: fixed column list (may include computed columns that have a schema `format`).
  // beforeLoad: async hook run alongside every load/refresh (e.g. to reload lookup data).
  // onCreated(record): called after "+ Add" saves a new record.
  // beforeDelete(record): async hook run after the user confirms a delete, before the record is removed.
  //   It may return an undo function, which is called if the delete then fails.
  function crudPage(resource, { mount, toolbarExtra = "", source = () => "", rowActions, onLoaded, sort, columns, beforeLoad, onCreated, beforeDelete } = {}) {
    const sch = SCHEMA[resource];
    mount.insertAdjacentHTML("beforeend", `
      <div class="card"><div class="card-body">
        <div class="d-flex flex-wrap gap-2 mb-3 align-items-center">
          <input class="form-control form-control-sm search" style="max-width:260px" placeholder="Search…">
          ${toolbarExtra}
          <span class="count text-muted small ms-1"></span>
          <div class="ms-auto d-flex gap-2">
            <button class="btn btn-sm btn-outline-secondary refresh" type="button">Refresh</button>
            <button class="btn btn-sm btn-primary add" type="button">+ Add ${esc(sch.label)}</button>
          </div>
        </div>
        <div class="table-host"></div>
      </div></div>`);
    const host = mount.querySelector(".table-host");
    const search = mount.querySelector(".search");
    const count = mount.querySelector(".count");
    let rows = [];

    const draw = () => {
      const shown = filterRows(rows, search.value);
      count.textContent = rows.length ? `${shown.length} of ${rows.length}` : "";
      renderTable(host, resource, shown, {
        columns,
        emptyText: rows.length ? "Nothing matches your search." : `No ${sch.labelPlural.toLowerCase()} yet. Use "+ Add ${sch.label}" to create one.`,
        onEdit: async r => { if (await openRecordForm(resource, r, rows)) load(); },
        onDelete: async r => { if (await confirmDelete(resource, r, beforeDelete)) load(); },
        rowActions
      });
    };
    async function load() {
      host.innerHTML = spinner();
      try {
        const [list] = await Promise.all([api.list(resource, source()), ensureRefs(resource), beforeLoad?.()]);
        rows = sort ? sort(list) : list; draw(); onLoaded?.(rows);
      }
      catch (e) { host.innerHTML = ""; host.appendChild(errorBox(e, load)); count.textContent = ""; }
    }
    search.addEventListener("input", draw);
    mount.querySelector(".refresh").onclick = load;
    mount.querySelector(".add").onclick = async () => {
      const saved = await openRecordForm(resource, null, rows);
      if (!saved) return;
      await load();
      onCreated?.(saved);
    };
    load();
    return { reload: load, rows: () => rows };
  }

  // An exception is in force when it's active and not past its expiration date
  const isLiveException = x => !!x && x.active !== false && (!x.expirationDate || parseDate(x.expirationDate) > new Date());
  // The live whole-host exception for a host, if any
  const hostException = (hostExceptions, hostId) => (hostExceptions || []).find(x => x.apiHostId === hostId && isLiveException(x)) || null;

  // ---------- apps ----------
  // Roll up each app's hosts and endpoints. An app's endpoints are every endpoint whose apiHostId is one of the
  // app's linked hosts; a host linked twice is only counted once. Links to missing apps/hosts are ignored.
  // Returns [{ app, hosts, endpoints, total, active, lastAudit }] in the same order as `apps`.
  function appRollup(apps, links, hosts, endpoints) {
    const hostById = new Map((hosts || []).map(h => [h.id, h]));
    const epsByHost = new Map();
    (endpoints || []).forEach(e => { if (e.apiHostId != null) (epsByHost.get(e.apiHostId) || epsByHost.set(e.apiHostId, []).get(e.apiHostId)).push(e); });
    return (apps || []).map(app => {
      const hostIds = [...new Set((links || []).filter(l => l.applicationId === app.id).map(l => l.apiHostId))].filter(id => hostById.has(id));
      const hs = hostIds.map(id => hostById.get(id));
      const eps = hostIds.flatMap(id => epsByHost.get(id) || []);
      const lastAudit = eps.concat(hs).reduce((m, x) => Math.max(m, dateMs(x.lastAuditDate)), 0);
      return { app, hosts: hs, endpoints: eps, total: eps.length, active: eps.filter(e => e.isActive !== false).length, lastAudit: lastAudit || null };
    });
  }
  // Remove ApplicationApi links matching `test` (e.g. before deleting an application or host, since the
  // foreign keys don't cascade). Returns an undo function that re-creates them. A 404 means the routes aren't deployed.
  async function removeAppLinks(test) {
    let links = [];
    try { links = (await api.list("apphosts")).filter(test); }
    catch (e) { if (e.status !== 404) throw e; }
    const removed = [];
    try { for (const l of links) { await api.remove("apphosts", l.id); removed.push(l); } }
    catch (e) { await Promise.all(removed.map(l => api.create("apphosts", { applicationId: l.applicationId, apiHostId: l.apiHostId }))); throw e; }
    const undo = () => Promise.all(removed.map(l => api.create("apphosts", { applicationId: l.applicationId, apiHostId: l.apiHostId })));
    undo.count = removed.length;
    return undo;
  }
  // Hosts not linked to any app
  const unlinkedHosts = (hosts, links, apps) => {
    const appIds = new Set((apps || []).map(a => a.id));
    const linked = new Set((links || []).filter(l => appIds.has(l.applicationId)).map(l => l.apiHostId));
    return (hosts || []).filter(h => !linked.has(h.id));
  };


  // ---------- Enterprise(9) logging standard ----------
  // An endpoint is Enterprise(9) if its PATH has "log" in it (never the host name: "capitoltechnology" contains "log"),
  // ignoring words that only contain "log" (login, logout, catalog... config.js enterpriseLogs.exclude), or if its path
  // names one of the standard sets in config.js (so /api/Usernotices and /api/userlocation/ count too).
  // Endpoints are grouped into sets: /api/ApiLog, /api/ApiLog/{id} and POST /api/ApiLog are the "ApiLog" set.
  const e9 = (() => {
    const LOGS = CFG.enterpriseLogs || { types: [] };
    const EXCLUDE = (LOGS.exclude || ["login", "logon", "logout", "logoff", "catalog", "dialog", "blog", "analog", "technolog", "backlog", "apolog"]).map(x => x.toLowerCase());
    const GENERIC = /^(log|logs|logging|logger|enterpriselog|enterpriselogs)$/i;   // /api/Logs/Learnlog -> "Learnlog"
    const blankV = v => v == null || String(v).trim() === "" || String(v).trim().toLowerCase() === "string";
    const pathOf = e => { const u = blankV(e.url) ? "" : String(e.url).trim(); try { return /^https?:\/\//i.test(u) ? new URL(u).pathname : u; } catch { return u; } };
    const segsOf = e => pathOf(e).split("/").filter(Boolean);
    const isParam = seg => /^\{.*\}$/.test(seg);
    const isLogWord = seg => /log/i.test(seg) && !isParam(seg) && !EXCLUDE.some(x => seg.toLowerCase().includes(x));
    const routeSeg = t => String(t.path || "").split("/").filter(Boolean).pop() || t.name;
    const namesOf = t => [...new Set([t.name, ...(t.aliases || []), routeSeg(t)].map(n => n.toLowerCase()))];
    const cfgFor = name => (LOGS.types || []).find(t => namesOf(t).includes(String(name).toLowerCase()));
    function setOf(e) {
      const segs = segsOf(e);
      for (const seg of segs) { if (isParam(seg)) continue; const t = cfgFor(seg); if (t) return t.name; }
      const i = segs.findIndex(isLogWord);
      if (i < 0) return null;
      if (GENERIC.test(segs[i])) return segs.slice(i + 1).find(x => !isParam(x)) || segs[i];
      return segs[i];
    }
    // [{ name, label, path, endpoints }] for the given endpoints, sorted by name
    function sets(endpoints) {
      const m = new Map();
      for (const e of endpoints || []) {
        const name = setOf(e); if (!name) continue;
        const cfg = cfgFor(name), k = (cfg?.name || name).toLowerCase();
        if (!m.has(k)) m.set(k, { name: cfg?.name || name, label: cfg?.label, path: cfg?.path, endpoints: [] });
        m.get(k).endpoints.push(e);
      }
      return [...m.values()].sort((a, b) => a.name.localeCompare(b.name));
    }
    return { setOf, sets, pathOf, segsOf, isParam, cfgFor, namesOf };
  })();
  window.Cocky = { e9, appRollup, unlinkedHosts, removeAppLinks, CFG, SCHEMA, PAGES, api, layout, toast, errorBox, spinner, renderTable, filterRows, openRecordForm, confirmDelete, crudPage, esc, titleCase, fmtValue, parseDate, dateMs, ensureRefs, refText, isLiveException, hostException };
})();
