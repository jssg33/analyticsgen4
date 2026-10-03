/*
 * cockyciphers.js — Caesar shifts over ASCII / byte values (CockyAuditor build 6.1)
 *
 * Unlike a textbook Caesar cipher (A–Z only), these shift every character's code value:
 * the text is turned into UTF-8 bytes (plain ASCII characters keep their ASCII code, 0–127),
 * each byte is shifted by N and wrapped at 256, and the result is returned as JSON with the
 * bytes in base64. Any string works, however long, including accented letters and emoji,
 * and decrypting gives back exactly the original.
 *
 *   encrypt3("Hello")  -> '{"cipher":"caesar3","shift":3,"range":256,"encoding":"base64",...,"data":"S2hvb3I="}'
 *   decrypt3(json)     -> "Hello"
 *
 *   encrypt3 / decrypt3, encrypt7 / decrypt7, encrypt96 / decrypt96. Each decryptN only accepts JSON made with
 *   shift N. encrypt(text, n) / decrypt(json) work with any shift (decrypt reads the shift from the JSON).
 *
 * Note: a Caesar shift hides text from a casual look only. Anyone can undo it, so don't use it to protect
 * passwords, tokens or personal data; use TLS for transport and AES / bcrypt on the server for that.
 *
 * CockyCiphers.store: CockyAuditor's localStorage, every value stored as encrypt96 JSON under the "cocky." key prefix (get / set / remove / getJSON / setJSON).
 *
 * Browser: <script src="js/cockyciphers.js"></script> -> window.CockyCiphers
 * Node:    const CockyCiphers = require("./js/cockyciphers.js");
 */
(function (root, factory) {
  const lib = factory();
  if (typeof module === "object" && module.exports) module.exports = lib;
  else root.CockyCiphers = lib;
})(typeof self !== "undefined" ? self : this, function () {
  "use strict";

  const VERSION = "1.0";
  const RANGE = 256;                 // byte values 0-255 (ASCII is 0-127)
  const CHUNK = 0x8000;              // bytes per String.fromCharCode call, so long strings don't overflow the stack

  const enc = new TextEncoder();
  const dec = new TextDecoder("utf-8", { fatal: true });

  // ---------- base64 for byte arrays (browser and Node) ----------
  function toBase64(bytes) {
    if (typeof Buffer !== "undefined") return Buffer.from(bytes.buffer, bytes.byteOffset, bytes.length).toString("base64");
    let bin = "";
    for (let i = 0; i < bytes.length; i += CHUNK) bin += String.fromCharCode.apply(null, bytes.subarray(i, i + CHUNK));
    return btoa(bin);
  }
  function fromBase64(b64) {
    if (typeof Buffer !== "undefined") return new Uint8Array(Buffer.from(b64, "base64"));
    const bin = atob(b64), out = new Uint8Array(bin.length);
    for (let i = 0; i < bin.length; i++) out[i] = bin.charCodeAt(i);
    return out;
  }

  // ---------- core ----------
  // Shift every byte by `shift` (any integer, negative to undo), wrapping at 256
  function shiftBytes(bytes, shift) {
    const s = ((shift % RANGE) + RANGE) % RANGE, out = new Uint8Array(bytes.length);
    for (let i = 0; i < bytes.length; i++) out[i] = (bytes[i] + s) & 0xff;
    return out;
  }

  // Encrypt `text` with any shift; returns the JSON string (or the object with { asObject: true })
  function encrypt(text, shift, opts = {}) {
    if (text == null) text = "";
    if (typeof text !== "string") text = String(text);
    if (!Number.isInteger(shift)) throw new TypeError("shift must be a whole number");
    const bytes = enc.encode(text);
    const payload = {
      cipher: opts.name || "caesar" + shift,
      version: VERSION,
      shift,
      range: RANGE,
      encoding: "base64",
      length: text.length,        // characters (UTF-16 code units) in the original
      bytes: bytes.length,        // UTF-8 bytes in the original
      createdAt: new Date().toISOString(),
      data: toBase64(shiftBytes(bytes, shift))
    };
    return opts.asObject ? payload : JSON.stringify(payload);
  }

  // Decrypt the JSON (string or object) made by encrypt / encryptN. `expectShift`: refuse JSON made with another shift.
  function decrypt(json, expectShift) {
    let p;
    try { p = typeof json === "string" ? JSON.parse(json) : json; } catch { throw new Error("Not valid JSON"); }
    if (!p || typeof p.data !== "string" || !Number.isInteger(p.shift)) throw new Error("Not a CockyCiphers payload (needs shift and data)");
    if (expectShift != null && p.shift !== expectShift) throw new Error(`This JSON was encrypted with shift ${p.shift}; use decrypt${p.shift}`);
    if (p.range != null && p.range !== RANGE) throw new Error(`Unsupported range ${p.range} (this library uses ${RANGE})`);
    if (p.encoding && p.encoding !== "base64") throw new Error(`Unsupported encoding ${p.encoding}`);
    const bytes = shiftBytes(fromBase64(p.data), -p.shift);
    if (p.bytes != null && bytes.length !== p.bytes) throw new Error(`Payload is damaged: expected ${p.bytes} bytes, got ${bytes.length}`);
    let text;
    try { text = dec.decode(bytes); } catch { throw new Error("Payload is damaged or the shift is wrong (not valid UTF-8 after shifting back)"); }
    return text;
  }

  const encrypt3 = (text, opts) => encrypt(text, 3, { ...opts, name: "caesar3" });
  const decrypt3 = json => decrypt(json, 3);
  const encrypt7 = (text, opts) => encrypt(text, 7, { ...opts, name: "caesar7" });
  const decrypt7 = json => decrypt(json, 7);
  const encrypt96 = (text, opts) => encrypt(text, 96, { ...opts, name: "caesar96" });
  const decrypt96 = json => decrypt(json, 96);

  // ---------- store: localStorage with every value written through encrypt96 ----------
  // CockyAuditor reads and writes localStorage only through this. Values are caesar96 JSON.
  // Build 6.7: every key lives under the "cocky." prefix (get("token") reads "cocky.token"), and the store never touches
  // a key outside that prefix. localStorage is shared by every page on the same origin, and builds 6.1-6.6 used bare
  // names (token, fullname, ...) and encrypted EVERY plain value on the origin, so another app on the same site found its
  // own values replaced with caesar96 JSON (and showed it on screen). migrateAll() moves CockyAuditor's old values under
  // the prefix and removes the encrypted copies from the bare names; plain values under bare names are left alone.
  // Like the cipher itself, this keeps values from being read at a glance; it is not protection against someone
  // who has this file.
  const ls = () => { try { return typeof localStorage !== "undefined" ? localStorage : null; } catch { return null; } };
  const isPayload = raw => { try { const p = JSON.parse(raw); return !!p && typeof p === "object" && p.shift === 96 && typeof p.data === "string"; } catch { return false; } };
  const PREFIX = "cocky.";
  const ns = key => String(key).startsWith(PREFIX) ? String(key) : PREFIX + key;
  // The bare names builds 6.1-6.6 wrote
  const LEGACY = ["userid", "token", "role", "username", "email", "fullname", "testlogin", "sessionid"];
  // Decrypt, unwrapping a value that was encrypted more than once (another tab with an older build re-saving it)
  const unwrap = raw => { let v = decrypt96(raw); for (let i = 0; i < 5 && isPayload(v); i++) v = decrypt96(v); return v; };
  const store = {
    PREFIX, key: ns,
    get(key) {
      const s = ls(); if (!s) return null;
      const k = ns(key);
      let raw; try { raw = s.getItem(k); } catch { return null; }
      if (raw == null) return null;
      if (isPayload(raw)) {
        try { const v = unwrap(raw); if (decrypt96(raw) !== v) s.setItem(k, encrypt96(v)); return v; } catch { return null; }
      }
      try { s.setItem(k, encrypt96(raw)); } catch {}     // a plain value under our own prefix: encrypt it
      return raw;
    },
    set(key, value) {
      const s = ls(); if (!s) return;
      let v = value == null ? null : String(value);
      if (v != null && isPayload(v)) { try { v = unwrap(v); } catch {} }   // never store an already-encrypted value
      try { v == null ? s.removeItem(ns(key)) : s.setItem(ns(key), encrypt96(v)); } catch {}
    },
    remove(key) { const s = ls(); if (!s) return; try { s.removeItem(ns(key)); } catch {} },
    getJSON(key, fallback = null) { const v = store.get(key); if (v == null || v === "") return fallback; try { return JSON.parse(v); } catch { return fallback; } },
    setJSON(key, value) { store.set(key, JSON.stringify(value)); },
    // Run once per page load by auth.js. Touches only CockyAuditor's keys:
    //  - "cocky.*" values still in plain text are encrypted (and double-encrypted ones unwrapped)
    //  - an old bare-name key (token, fullname, ...) holding caesar96 JSON was written by CockyAuditor 6.1-6.6: its value is
    //    moved to "cocky.<name>" (unless that's already set) and the bare key is removed, so other apps on this origin
    //    stop seeing encrypted values. A plain value under a bare name belongs to someone else and is left alone.
    // Returns how many keys it changed.
    migrateAll() {
      const s = ls(); if (!s) return 0; let n = 0;
      try {
        for (const k of LEGACY) {
          const raw = s.getItem(k);
          if (raw == null || !isPayload(raw)) continue;
          try { if (s.getItem(ns(k)) == null) s.setItem(ns(k), encrypt96(unwrap(raw))); } catch {}
          s.removeItem(k); n++;
        }
        const keys = []; for (let i = 0; i < s.length; i++) keys.push(s.key(i));
        for (const k of keys) {
          if (!k || !k.startsWith(PREFIX)) continue;
          const raw = s.getItem(k);
          if (raw == null) continue;
          if (!isPayload(raw)) { s.setItem(k, encrypt96(raw)); n++; }
          else { try { const v = unwrap(raw); if (decrypt96(raw) !== v) { s.setItem(k, encrypt96(v)); n++; } } catch {} }
        }
      } catch {}
      return n;
    }
  };

  return { VERSION, RANGE, encrypt3, decrypt3, encrypt7, decrypt7, encrypt96, decrypt96, encrypt, decrypt, shiftBytes, toBase64, fromBase64, store };
});
