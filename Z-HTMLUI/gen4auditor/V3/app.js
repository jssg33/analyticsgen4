// CockyAuditor shared code: layout, API client, tables, add/edit dialog.
(function () {
  const CFG = window.COCKY_CONFIG;
  const SCHEMA = window.COCKY_SCHEMA;

  const PAGES = [
    { key: "dashboard",  href: "apidashboard.html",  label: "Dashboard" },
    { key: "hosts",      href: "apihosts.html",      label: "Hosts" },
    { key: "endpoints",  href: "apiendpoints.html",  label: "Endpoints" },
    { key: "audit",      href: "apiaudit.html",      label: "Run Audit" },
    { key: "exceptions", href: "apiexceptions.html", label: "Exceptions" },
    { key: "history",    href: "apihistory.html",    label: "History" }
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
  const refLabel = r => `#${r.id} ${r.description || r.apiHostName || r.name || ""}`.trim();
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
          <div class="api-note" title="${esc(CFG.apiBaseUrl)}">API: ${esc(new URL(CFG.apiBaseUrl).host.split(".")[0])}</div>
        </nav>
        <main class="content" id="page"></main>
      </div>
      <div class="toast-container position-fixed bottom-0 end-0 p-3" id="toasts"></div>`);
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
        <thead><tr>${cols.map(c => `<th>${esc(titleCase(c))}</th>`).join("")}${actions ? "<th></th>" : ""}</tr></thead>
        <tbody>${rows.map((r, i) => `<tr data-i="${i}" class="${opts.onRowClick ? "clickable" : ""}">
          ${cols.map(c => `<td>${SCHEMA[resource]?.format?.[c] ? SCHEMA[resource].format[c](r[c], r) : fmtValue(r[c], fieldType(resource, c, r[c]))}</td>`).join("")}
          ${actions ? `<td class="text-end text-nowrap">
            ${(opts.rowActions || []).map((a, ai) => `<button class="btn btn-sm btn-outline-primary me-1" data-act="x${ai}">${esc(a.label)}</button>`).join("")}
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
  async function openRecordForm(resource, record, sampleRows = []) {
    await ensureRefs(resource);
    return new Promise(resolve => {
      const m = ensureModal();
      const isNew = !record;
      const sch = SCHEMA[resource] || { fields: {} };
      const learned = sampleRows.length ? Object.keys(Object.assign({}, ...sampleRows)) : [];
      const keys = [...new Set([...(record ? Object.keys(record) : learned.length ? learned : Object.keys(sch.fields))])];
      const sample = record || Object.assign({}, ...sampleRows);
      const base = {};
      if (isNew) keys.forEach(k => { if (k !== "id") base[k] = fieldType(resource, k, sample[k]) === "bool" ? /^(is)?active$/i.test(k) : null; });

      m.querySelector(".modal-title").textContent = `${isNew ? "Add" : "Edit"} ${sch.label || resource}${record ? " #" + record.id : ""}`;
      const note = m.querySelector(".guess-note");
      const guessing = !sch.verified && !learned.length && !record;
      note.classList.toggle("d-none", !guessing);
      note.textContent = guessing
        ? `There are no ${(sch.labelPlural || resource).toLowerCase()} yet, so these fields are a best guess. Compare with the Swagger schema; if the save fails, adjust in the JSON tab (and in schema.js).`
        : "";
      const form = m.querySelector(".tab-form");
      form.innerHTML = keys.map(k => inputFor(k, fieldType(resource, k, sample[k]), record ? record[k] : base[k], isNew)).join("");
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
          const saved = isNew ? await api.create(resource, body) : await api.update(resource, record.id, body);
          bs.hide(); saved_ = saved ?? body;
          toast(`${sch.label || "Record"} ${isNew ? "added" : "saved"}.`);
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

  async function confirmDelete(resource, row) {
    const sch = SCHEMA[resource] || {};
    const name = row.description || row.hostName || row.name || "";
    if (!confirm(`Delete ${sch.label || "record"} #${row.id}${name ? ` (${name})` : ""}? This can't be undone.`)) return false;
    try {
      await api.remove(resource, row.id);
      toast(`${sch.label || "Record"} #${row.id} deleted.`);
      return true;
    } catch (e) { toast(e.message, "danger"); return false; }
  }

  // ---------- full CRUD page ----------
  // Used by Hosts / Endpoints / Exceptions. `source()` returns the list suffix to load.
  function crudPage(resource, { mount, toolbarExtra = "", source = () => "", rowActions, onLoaded, sort } = {}) {
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
        emptyText: rows.length ? "Nothing matches your search." : `No ${sch.labelPlural.toLowerCase()} yet. Use "+ Add ${sch.label}" to create one.`,
        onEdit: async r => { if (await openRecordForm(resource, r, rows)) load(); },
        onDelete: async r => { if (await confirmDelete(resource, r)) load(); },
        rowActions
      });
    };
    async function load() {
      host.innerHTML = spinner();
      try {
        const [list] = await Promise.all([api.list(resource, source()), ensureRefs(resource)]);
        rows = sort ? sort(list) : list; draw(); onLoaded?.(rows);
      }
      catch (e) { host.innerHTML = ""; host.appendChild(errorBox(e, load)); count.textContent = ""; }
    }
    search.addEventListener("input", draw);
    mount.querySelector(".refresh").onclick = load;
    mount.querySelector(".add").onclick = async () => { if (await openRecordForm(resource, null, rows)) load(); };
    load();
    return { reload: load, rows: () => rows };
  }

  window.Cocky = { CFG, SCHEMA, PAGES, api, layout, toast, errorBox, spinner, renderTable, filterRows, openRecordForm, confirmDelete, crudPage, esc, titleCase, fmtValue, parseDate, dateMs, ensureRefs, refText };
})();
