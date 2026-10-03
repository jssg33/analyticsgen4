// CockyAuditor user groups (build 5.8). Load after app.js.
// /api/Usergroups = the groups (Usergroup: id, groupid, groupdescription, groupownerid, groupcompanyid).
// A user's groups live in the User record's slots (config.js groups.userSlots: groupid1..groupid5), each holding a group's
// Groupid code. A slot that holds the group's numeric id or its description is still recognised.
(function () {
  const { api, esc, CFG } = window.Cocky;
  const GC = Object.assign({ userSlots: ["groupid1", "groupid2", "groupid3", "groupid4", "groupid5"], companyCheck: true }, CFG.groups || {});
  const lc = v => String(v ?? "").trim().toLowerCase();
  const blank = v => v == null || ["", "string", "null", "0"].includes(lc(v));
  const pick = (o, name) => { if (!o) return undefined; const k = Object.keys(o).find(x => lc(x).replace(/_/g, "") === lc(name).replace(/_/g, "")); return k === undefined ? undefined : o[k]; };
  const str = (o, name) => { const v = pick(o, name); return blank(v) ? "" : String(v).trim(); };

  // ---------- groups ----------
  const code = g => str(g, "groupid") || String(g.id);
  const label = g => str(g, "groupdescription") || str(g, "groupid") || "Group #" + g.id;
  const company = g => str(g, "groupcompanyid");
  const ownerId = g => { const v = pick(g, "groupownerid"); return blank(v) ? null : Number(v); };
  // The group a slot value names: by Groupid code, then numeric id, then description
  function match(value, groups) {
    const v = lc(value); if (!v) return null;
    return groups.find(g => lc(str(g, "groupid")) === v) || groups.find(g => String(g.id) === v) || groups.find(g => lc(str(g, "groupdescription")) === v) || null;
  }

  // ---------- user slots ----------
  // The JSON spelling of each slot on the user records (groupid1 / groupId1 / GroupId1)
  function slotKeys(users) {
    return GC.userSlots.map(s => { for (const u of users || []) { const k = Object.keys(u).find(x => lc(x) === lc(s)); if (k) return k; } return s; });
  }
  const hasSlots = users => (users || []).some(u => GC.userSlots.some(s => Object.keys(u).some(x => lc(x) === lc(s))));
  // [{ slot, value, group }] for the filled slots
  function slotsOf(u, groups, keys) {
    return (keys || slotKeys([u])).map(k => ({ slot: k, value: blank(u[k]) ? "" : String(u[k]).trim() })).filter(x => x.value)
      .map(x => ({ ...x, group: match(x.value, groups) }));
  }
  const groupsOf = (u, groups, keys) => slotsOf(u, groups, keys).map(x => x.group).filter(Boolean);
  const isMember = (u, g, groups, keys) => groupsOf(u, groups, keys).some(x => x.id === g.id);
  // The slot values for a list of groups: codes in order, the rest null. Slots naming a group that doesn't exist are kept
  // (so nothing is lost silently) unless dropUnknown is set.
  function slotPayload(u, list, groups, keys, dropUnknown = false) {
    const unknown = dropUnknown ? [] : slotsOf(u, groups, keys).filter(x => !x.group).map(x => x.value);
    const vals = [...new Set([...list.map(code), ...unknown])];
    if (vals.length > keys.length) throw new Error(`A user can be in at most ${keys.length} groups.`);
    return Object.fromEntries(keys.map((k, i) => [k, vals[i] ?? null]));
  }
  // Save a user's groups. Sends the registration fields and roles with the slots, like Edit user does; the API's PUT leaves
  // fields it isn't sent unchanged, and passwords are never sent.
  async function saveUserGroups(u, list, groups, keys) {
    const slots = slotPayload(u, list, groups, keys);
    const keep = {};
    Object.keys(u).forEach(k => { if (/^(id|firstname|lastname|fullname|username|email|role\d*)$/i.test(k)) keep[k] = u[k]; });
    await api.update("users", u.id, { ...keep, ...slots });
    Object.assign(u, slots);
  }

  // ---------- findings ----------
  // For the Users page: [{ level, text }]
  function userFindings(u, groups, keys) {
    const f = [];
    slotsOf(u, groups, keys).forEach(x => {
      if (!x.group) f.push({ level: "warn", text: `${x.slot} = "${x.value}" doesn't match any group` });
      else if (lc(x.value) !== lc(code(x.group))) f.push({ level: "info", text: `${x.slot} holds "${x.value}" instead of the group code "${code(x.group)}"` });
    });
    const mine = groupsOf(u, groups, keys), dup = mine.filter((g, i) => mine.findIndex(x => x.id === g.id) !== i);
    if (dup.length) f.push({ level: "info", text: `In group "${label(dup[0])}" more than once` });
    if (GC.companyCheck) {
      const uc = str(u, "companyid");
      mine.filter(g => company(g) && uc && lc(company(g)) !== lc(uc))
        .forEach(g => f.push({ level: "warn", text: `In group "${label(g)}" of company ${company(g)}, but the user's company is ${uc}` }));
    }
    return f;
  }
  // For the Groups page
  function groupFindings(g, groups, users, keys) {
    const f = [], members = users.filter(u => isMember(u, g, groups, keys));
    if (!str(g, "groupid")) f.push({ level: "warn", text: "No Groupid code; users' slots will store its numeric id instead" });
    if (groups.some(x => x.id !== g.id && str(g, "groupid") && lc(str(x, "groupid")) === lc(str(g, "groupid")))) f.push({ level: "fail", text: `Groupid "${str(g, "groupid")}" is used by another group too` });
    const oid = ownerId(g);
    if (oid == null) f.push({ level: "warn", text: "No group owner" });
    else if (!users.some(u => u.id === oid)) f.push({ level: "warn", text: `Owner user #${oid} doesn't exist` });
    if (!members.length) f.push({ level: "info", text: "No members" });
    if (GC.companyCheck && company(g)) {
      const other = members.filter(u => str(u, "companyid") && lc(str(u, "companyid")) !== lc(company(g)));
      if (other.length) f.push({ level: "warn", text: `${other.length} member(s) from another company` });
    }
    return f;
  }

  // ---------- dialogs ----------
  function modal(title, bodyHtml, okText) {
    document.getElementById("grpModal")?.remove();
    document.body.insertAdjacentHTML("beforeend", `<div class="modal fade" id="grpModal" tabindex="-1"><div class="modal-dialog modal-lg modal-dialog-scrollable"><div class="modal-content">
      <div class="modal-header"><h5 class="modal-title">${esc(title)}</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
      <div class="modal-body">${bodyHtml}<div class="alert alert-danger small mt-2 mb-0 d-none grp-err"></div></div>
      <div class="modal-footer justify-content-between"><div class="small text-muted grp-sum"></div>
        <div><button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Cancel</button>
        <button class="btn btn-primary grp-ok" type="button">${esc(okText)}</button></div></div></div></div></div>`);
    const el = document.getElementById("grpModal");
    return { el, m: bootstrap.Modal.getOrCreateInstance(el), err: msg => { const e = el.querySelector(".grp-err"); e.textContent = msg; e.classList.toggle("d-none", !msg); } };
  }
  const userName = u => u.fullname || [u.firstname, u.lastname].filter(Boolean).join(" ") || u.username || u.email || "User #" + u.id;

  // A user's groups: tick up to five. Resolves true when saved.
  function editUserGroups(u, groups, users) {
    const keys = slotKeys(users), max = keys.length;
    const picked = new Set(groupsOf(u, groups, keys).map(g => g.id));
    const unknown = slotsOf(u, groups, keys).filter(x => !x.group);
    const sorted = groups.slice().sort((a, b) => label(a).localeCompare(label(b)));
    const d = modal(`Groups for ${userName(u)}`, `
      <input class="form-control form-control-sm mb-2 grp-q" placeholder="Search groups…">
      ${unknown.length ? `<div class="alert alert-warning small py-2">Slot(s) naming a group that doesn't exist: ${unknown.map(x => `<code>${esc(x.slot)}</code> = ${esc(x.value)}`).join(", ")}.
        <div class="form-check mt-1"><input class="form-check-input grp-drop" type="checkbox" id="grpDrop"><label class="form-check-label" for="grpDrop">Clear them when saving</label></div></div>` : ""}
      <div class="grp-list"></div>`, "Save groups");
    const draw = () => {
      const q = lc(d.el.querySelector(".grp-q").value);
      const shown = sorted.filter(g => !q || [label(g), code(g), company(g)].some(v => lc(v).includes(q)));
      d.el.querySelector(".grp-list").innerHTML = shown.length ? `<div class="list-group">${shown.map(g => {
        const n = users.filter(x => isMember(x, g, groups, keys)).length, owner = users.find(x => x.id === ownerId(g));
        const full = !picked.has(g.id) && picked.size >= max;
        return `<label class="list-group-item d-flex gap-3 align-items-start ${full ? "text-muted" : ""}">
          <input class="form-check-input mt-1" type="checkbox" value="${g.id}" ${picked.has(g.id) ? "checked" : ""} ${full ? "disabled" : ""}>
          <span class="flex-grow-1"><b>${esc(label(g))}</b> <code class="small">${esc(code(g))}</code>
            ${company(g) ? `<span class="badge text-bg-light border fw-normal ms-1">Company ${esc(company(g))}</span>` : ""}
            <br><span class="small text-muted">${owner ? "Owner: " + esc(userName(owner)) + " · " : ""}${n} member${n === 1 ? "" : "s"}</span></span></label>`; }).join("")}</div>`
        : `<div class="empty">${groups.length ? "No groups match." : 'No groups yet. Add them on the <a href="groups.html">Groups</a> page.'}</div>`;
      d.el.querySelectorAll(".grp-list input").forEach(cb => cb.onchange = () => { cb.checked ? picked.add(+cb.value) : picked.delete(+cb.value); draw(); });
      d.el.querySelector(".grp-sum").textContent = `${picked.size} of ${max} groups`;
    };
    d.el.querySelector(".grp-q").oninput = draw;
    draw();
    return new Promise(res => {
      let saved = false;
      d.el.querySelector(".grp-ok").onclick = async () => {
        const btn = d.el.querySelector(".grp-ok"); btn.disabled = true; d.err("");
        try {
          const list = sorted.filter(g => picked.has(g.id));
          const drop = d.el.querySelector(".grp-drop")?.checked;
          const slots = slotPayload(u, list, groups, keys, drop);
          const keep = {}; Object.keys(u).forEach(k => { if (/^(id|firstname|lastname|fullname|username|email|role\d*)$/i.test(k)) keep[k] = u[k]; });
          await api.update("users", u.id, { ...keep, ...slots }); Object.assign(u, slots);
          saved = true; d.m.hide();
        } catch (e) { d.err(e.message); btn.disabled = false; }
      };
      d.el.addEventListener("hidden.bs.modal", () => { d.el.remove(); res(saved); });
      d.m.show();
    });
  }

  // A group's members: tick users. Users already in five other groups can't be added. Resolves { added, removed, failures } or null.
  function editMembers(g, groups, users) {
    const keys = slotKeys(users), max = keys.length;
    const before = new Set(users.filter(u => isMember(u, g, groups, keys)).map(u => u.id)), picked = new Set(before);
    const sorted = users.slice().sort((a, b) => userName(a).localeCompare(userName(b)));
    const d = modal(`Members of ${label(g)} (${code(g)})`, `
      <div class="d-flex gap-2 mb-2"><input class="form-control form-control-sm grp-q" placeholder="Search users…">
        <select class="form-select form-select-sm grp-show" style="max-width:170px"><option value="">All users</option><option value="in">Members</option><option value="out">Not members</option></select></div>
      <div class="grp-list"></div>`, "Save members");
    const draw = () => {
      const q = lc(d.el.querySelector(".grp-q").value), show = d.el.querySelector(".grp-show").value;
      const shown = sorted.filter(u => (!q || [userName(u), u.username, u.email, str(u, "companyid")].some(v => lc(v).includes(q))) &&
        (!show || (show === "in") === picked.has(u.id)));
      d.el.querySelector(".grp-list").innerHTML = shown.length ? `<div class="list-group">${shown.map(u => {
        const others = groupsOf(u, groups, keys).filter(x => x.id !== g.id), unknown = slotsOf(u, groups, keys).filter(x => !x.group).length;
        const full = !picked.has(u.id) && others.length + unknown >= max;
        const otherCo = GC.companyCheck && company(g) && str(u, "companyid") && lc(str(u, "companyid")) !== lc(company(g));
        return `<label class="list-group-item d-flex gap-3 align-items-start ${full ? "text-muted" : ""}">
          <input class="form-check-input mt-1" type="checkbox" value="${u.id}" ${picked.has(u.id) ? "checked" : ""} ${full ? "disabled" : ""}>
          <span class="flex-grow-1"><b>${esc(userName(u))}</b> <span class="small text-muted">${esc(u.username || "")}${u.email ? " · " + esc(u.email) : ""}</span>
            ${otherCo ? `<span class="badge text-bg-warning fw-normal ms-1">Company ${esc(str(u, "companyid"))}</span>` : ""}
            <br><span class="small text-muted">${others.length ? "Also in: " + others.map(x => esc(label(x))).join(", ") : "No other groups"}${full ? ` · already in ${max} groups` : ""}</span></span></label>`; }).join("")}</div>`
        : '<div class="empty">No users match.</div>';
      d.el.querySelectorAll(".grp-list input").forEach(cb => cb.onchange = () => { cb.checked ? picked.add(+cb.value) : picked.delete(+cb.value); sum(); });
      sum();
    };
    const sum = () => {
      const add = [...picked].filter(id => !before.has(id)).length, rem = [...before].filter(id => !picked.has(id)).length;
      d.el.querySelector(".grp-sum").textContent = `${picked.size} member${picked.size === 1 ? "" : "s"}${add || rem ? ` · ${add} to add, ${rem} to remove` : ""}`;
    };
    d.el.querySelector(".grp-q").oninput = draw; d.el.querySelector(".grp-show").onchange = draw;
    draw();
    return new Promise(res => {
      let result = null;
      d.el.querySelector(".grp-ok").onclick = async () => {
        const btn = d.el.querySelector(".grp-ok"); btn.disabled = true; d.err("");
        const add = users.filter(u => picked.has(u.id) && !before.has(u.id)), rem = users.filter(u => !picked.has(u.id) && before.has(u.id));
        const out = { added: 0, removed: 0, failures: [] };
        for (const u of add) {
          try { await saveUserGroups(u, [...groupsOf(u, groups, keys).filter(x => x.id !== g.id), g], groups, keys); out.added++; }
          catch (e) { out.failures.push(`${userName(u)}: ${e.message}`); }
        }
        for (const u of rem) {
          try { await saveUserGroups(u, groupsOf(u, groups, keys).filter(x => x.id !== g.id), groups, keys); out.removed++; }
          catch (e) { out.failures.push(`${userName(u)}: ${e.message}`); }
        }
        result = out;
        if (out.failures.length) { d.err(`${out.failures.length} change(s) failed: ${out.failures.join("; ")}`); btn.disabled = false; return; }
        d.m.hide();
      };
      d.el.addEventListener("hidden.bs.modal", () => { d.el.remove(); res(result); });
      d.m.show();
    });
  }

  window.Groups = { GC, code, label, company, ownerId, match, slotKeys, hasSlots, slotsOf, groupsOf, isMember, slotPayload, saveUserGroups,
    userFindings, groupFindings, editUserGroups, editMembers, userName, str };
})();
