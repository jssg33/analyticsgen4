// CockyAuditor charts (build 5.4). D3 v7 is loaded from jsDelivr on the pages that chart:
//   <script src="https://cdn.jsdelivr.net/npm/d3@7/dist/d3.min.js"></script>
// Shared by the Dashboard (platform pies) and, later, the Performance page.
(function () {
  const esc = s => String(s ?? "").replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));

  // Categorical colors, assigned in this fixed order (never cycled). Unknown is always neutral gray.
  const SERIES = ["#2a78d6", "#eb6834", "#1baf7a", "#eda100", "#e87ba4", "#008300", "#4a3aa7", "#e34948"];
  const NEUTRAL = "#b4b2a9";

  // ---------- platform (NodeJS vs DotNet) ----------
  // config.js platforms: [{ name, match }] - the first rule whose regex matches a host's text wins.
  const CFG = window.COCKY_CONFIG || {};
  const P = Object.assign({ rules: [], fields: [], unknown: "Unknown", mixed: "Mixed" }, CFG.platforms || {});
  const rules = P.rules.map((r, i) => ({ ...r, re: new RegExp(r.match, "i"), color: r.color || SERIES[i] }));
  const lc = v => String(v ?? "").toLowerCase();
  const pick = (o, name) => { const k = Object.keys(o || {}).find(x => lc(x) === lc(name)); return k ? o[k] : undefined; };
  const textOf = o => P.fields.map(f => pick(o, f)).filter(v => v != null && v !== "" && lc(v) !== "string" && lc(v) !== "n/a").join(" ");
  const match = text => text ? rules.find(r => r.re.test(text)) : null;

  // Platform of one host: its own fields, then its endpoints' fields (most common answer), then back-end
  // interfaces found through SystemConsole (P.interfacesMean). Returns { name, how } where how says why.
  function hostPlatform(h, endpoints = [], interfaces = []) {
    const own = match(textOf(h));
    if (own) return { name: own.name, how: "host" };
    const votes = {};
    endpoints.filter(e => e.apiHostId === h.id).forEach(e => { const m = match(textOf(e)); if (m) votes[m.name] = (votes[m.name] || 0) + 1; });
    const best = Object.entries(votes).sort((a, b) => b[1] - a[1])[0];
    if (best) return { name: best[0], how: "endpoints" };
    if (P.interfacesMean && interfaces.some(x => x.apiHostId === h.id)) return { name: P.interfacesMean, how: "interfaces" };
    return { name: P.unknown, how: "" };
  }
  const colorFor = name => name === P.unknown ? NEUTRAL : name === P.mixed ? (P.mixedColor || SERIES[rules.length] || NEUTRAL)
    : (rules.find(r => r.name === name)?.color || NEUTRAL);
  // Slices in a stable order: configured platforms, then Mixed, then Unknown (so colors never follow rank)
  const order = name => name === P.unknown ? 1e3 : name === P.mixed ? 999 : Math.max(0, rules.findIndex(r => r.name === name));

  // ---------- donut ----------
  // el: container. data: [{ name, value, note? }]. opts: { title, unit, onSlice(name) }
  // Draws a donut with the total in the middle, a legend with counts and %, and a hover tooltip.
  let tip;
  function tooltip() {
    if (tip) return tip;
    tip = document.createElement("div");
    tip.className = "viz-tip"; tip.setAttribute("role", "status");
    document.body.appendChild(tip);
    return tip;
  }
  function donut(el, data, opts = {}) {
    const d3 = window.d3;
    const rows = data.filter(d => d.value > 0).sort((a, b) => order(a.name) - order(b.name));
    const total = d3 ? d3.sum(rows, d => d.value) : rows.reduce((s, d) => s + d.value, 0);
    const pct = v => total ? Math.round(v / total * 100) : 0;
    const unit = opts.unit || "";
    el.innerHTML = `<div class="viz-card">
      <div class="viz-title">${esc(opts.title || "")}</div>
      <div class="viz-body"><div class="viz-plot"></div>
        <ul class="viz-legend">${rows.map(d => `<li data-name="${esc(d.name)}"${opts.onSlice ? ' class="clickable"' : ""}>
          <span class="sw" style="background:${colorFor(d.name)}"></span><span class="nm">${esc(d.name)}</span>
          <span class="vl">${d.value}</span><span class="pc">${pct(d.value)}%</span></li>`).join("")}</ul></div>
      ${opts.note ? `<div class="viz-note">${opts.note}</div>` : ""}</div>`;
    const plot = el.querySelector(".viz-plot");
    if (!total) { plot.innerHTML = `<div class="viz-empty">No ${esc(unit || "data")} yet</div>`; return; }
    if (!d3) { plot.innerHTML = `<div class="viz-empty">Chart library (D3) didn't load</div>`; return; }

    const size = 168, r = size / 2, inner = r * 0.62;
    const svg = d3.select(plot).append("svg").attr("viewBox", `0 0 ${size} ${size}`).attr("width", size).attr("height", size)
      .attr("role", "img").attr("aria-label", `${opts.title || ""}: ` + rows.map(d => `${d.name} ${d.value}`).join(", "));
    const g = svg.append("g").attr("transform", `translate(${r},${r})`);
    const arcs = d3.pie().value(d => d.value).sort(null).padAngle(rows.length > 1 ? 0.012 : 0)(rows);
    const arc = d3.arc().innerRadius(inner).outerRadius(r - 4).cornerRadius(3);
    const arcHover = d3.arc().innerRadius(inner).outerRadius(r).cornerRadius(3);
    const t = tooltip();
    const show = (ev, d) => {
      t.innerHTML = `<b>${esc(d.data.name)}</b><br>${d.data.value} ${esc(unit)} · ${pct(d.data.value)}%${d.data.note ? `<div class="n">${esc(d.data.note)}</div>` : ""}`;
      t.style.display = "block";
      const x = Math.min(ev.clientX + 14, innerWidth - t.offsetWidth - 8), y = Math.min(ev.clientY + 14, innerHeight - t.offsetHeight - 8);
      t.style.left = x + "px"; t.style.top = y + "px";
    };
    const focus = name => {
      paths.transition().duration(120).attr("d", d => (d.data.name === name ? arcHover : arc)(d)).attr("opacity", d => !name || d.data.name === name ? 1 : 0.45);
      el.querySelectorAll(".viz-legend li").forEach(li => li.classList.toggle("dim", !!name && li.dataset.name !== name));
    };
    const paths = g.selectAll("path").data(arcs).join("path")
      .attr("d", arc).attr("fill", d => colorFor(d.data.name))
      .attr("stroke", "#fff").attr("stroke-width", 2)
      .style("cursor", opts.onSlice ? "pointer" : "default")
      .on("mousemove", (ev, d) => { show(ev, d); focus(d.data.name); })
      .on("mouseleave", () => { t.style.display = "none"; focus(null); })
      .on("click", (ev, d) => opts.onSlice?.(d.data.name));
    g.append("text").attr("class", "viz-total").attr("text-anchor", "middle").attr("dy", "0.1em").text(total);
    g.append("text").attr("class", "viz-unit").attr("text-anchor", "middle").attr("dy", "1.6em").text(unit);
    el.querySelectorAll(".viz-legend li").forEach(li => {
      li.onmouseenter = () => focus(li.dataset.name); li.onmouseleave = () => focus(null);
      if (opts.onSlice) li.onclick = () => opts.onSlice(li.dataset.name);
    });
  }

  // ---------- horizontal bars (build 5.6, Performance page) ----------
  // el: container. rows: [{ label, sub?, value, color?, tip? (html), href? }], longest first.
  // opts: { unit: "ms", format(v), rules: [{ value, label, color }] (dashed threshold lines), max }
  function hbars(el, rows, opts = {}) {
    const d3 = window.d3;
    el.innerHTML = "";
    if (!rows.length) { el.innerHTML = '<div class="empty">Nothing to chart.</div>'; return; }
    if (!d3) { el.innerHTML = '<div class="empty">Chart library (D3) didn\'t load.</div>'; return; }
    const fmt = opts.format || (v => v + (opts.unit ? " " + opts.unit : ""));
    const rowH = 26, gap = 6, top = 22, labelW = Math.min(300, Math.max(140, el.clientWidth * 0.34)), valueW = 70;
    const bottom = (opts.rules || []).length ? 22 : 4;   // threshold labels sit under the plot, clear of the tick labels
    const width = Math.max(420, el.clientWidth || 700), plotH = top + rows.length * (rowH + gap), height = plotH + bottom;
    const max = Math.max(opts.max || 0, d3.max(rows, d => d.value) || 1, ...(opts.rules || []).map(r => r.value * 1.08));
    const x = d3.scaleLinear().domain([0, max]).range([labelW, width - valueW]).nice();
    const svg = d3.select(el).append("svg").attr("viewBox", `0 0 ${width} ${height}`).attr("width", "100%").attr("class", "viz-hbars")
      .attr("role", "img").attr("aria-label", rows.map(d => `${d.label} ${fmt(d.value)}`).join(", "));
    // recessive grid + axis labels on top
    const ticks = x.ticks(5);
    svg.append("g").selectAll("line").data(ticks).join("line").attr("x1", d => x(d)).attr("x2", d => x(d)).attr("y1", top - 4).attr("y2", plotH)
      .attr("stroke", "#ecebe6");
    svg.append("g").selectAll("text").data(ticks).join("text").attr("class", "viz-axis").attr("x", d => x(d)).attr("y", 12).attr("text-anchor", "middle")
      .text(d => opts.tickFormat ? opts.tickFormat(d) : d);
    const t = tooltip();
    const g = svg.append("g").selectAll("g").data(rows).join("g").attr("transform", (d, i) => `translate(0,${top + i * (rowH + gap)})`)
      .style("cursor", d => d.href ? "pointer" : "default")
      .on("mousemove", (ev, d) => {
        t.innerHTML = d.tip || `<b>${esc(d.label)}</b><br>${esc(fmt(d.value))}`; t.style.display = "block";
        t.style.left = Math.min(ev.clientX + 14, innerWidth - t.offsetWidth - 8) + "px"; t.style.top = Math.min(ev.clientY + 14, innerHeight - t.offsetHeight - 8) + "px";
      })
      .on("mouseenter", function () { d3.select(this).select("rect.hit").attr("fill", "#f1f0ec"); })
      .on("mouseleave", function () { t.style.display = "none"; d3.select(this).select("rect.hit").attr("fill", "transparent"); })
      .on("click", (ev, d) => { if (d.href) location.href = d.href; });
    g.append("rect").attr("class", "hit").attr("x", 0).attr("y", -gap / 2).attr("width", width).attr("height", rowH + gap).attr("fill", "transparent").attr("rx", 4);
    const clip = (str, px) => { str = String(str ?? ""); const n = Math.floor(px / 6.6); return str.length > n ? str.slice(0, n - 1) + "…" : str; };
    g.append("text").attr("class", "viz-lbl").attr("x", labelW - 10).attr("y", d => d.sub ? 11 : rowH / 2 + 4).attr("text-anchor", "end").text(d => clip(d.label, labelW - 14));
    g.filter(d => d.sub).append("text").attr("class", "viz-sub").attr("x", labelW - 10).attr("y", 23).attr("text-anchor", "end").text(d => clip(d.sub, labelW - 14));
    // thin bar, 4px rounded end, anchored at the baseline
    g.append("path").attr("fill", d => d.color || SERIES[0]).attr("d", d => {
      const x0 = x(0), w = Math.max(2, x(d.value) - x0), y0 = 6, h = rowH - 12, r = Math.min(4, w / 2);
      return `M${x0},${y0}H${x0 + w - r}Q${x0 + w},${y0} ${x0 + w},${y0 + r}V${y0 + h - r}Q${x0 + w},${y0 + h} ${x0 + w - r},${y0 + h}H${x0}Z`;
    });
    g.append("text").attr("class", "viz-val").attr("x", d => x(d.value) + 6).attr("y", rowH / 2 + 4).text(d => fmt(d.value));
    // threshold rules
    (opts.rules || []).forEach(r => {
      if (r.value > x.domain()[1]) return;
      svg.append("line").attr("x1", x(r.value)).attr("x2", x(r.value)).attr("y1", top - 4).attr("y2", plotH + 4).attr("stroke", r.color || "#52514e")
        .attr("stroke-width", 1.5).attr("stroke-dasharray", "4 3");
      svg.append("text").attr("class", "viz-rule").attr("x", x(r.value)).attr("y", plotH + 17).attr("text-anchor", "middle").attr("fill", r.color || "#52514e").text(r.label);
    });
  }

  window.CockyCharts = { donut, hbars, hostPlatform, colorFor, platforms: P, SERIES, NEUTRAL };
})();
