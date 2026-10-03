# CockyAuditor

A static web front end for the audit API at
`https://cockyanalyticsg4-cqfrgacteud3c2h3.westus3-01.azurewebsites.net`.

## Version
Current: **Build 6.7 · Database fields & storage fix**. The version and build history live in `config.js` (`version`, `builds`);
add each new build to the top of `builds` and the home page and sidebar pick it up.

| Build | Title |
|---|---|
| 6.7 | Database fields & storage fix |
| 6.6 | Sortable grids, schema areas & token audit |
| 6.5 | Database Tables discovery |
| 6.4 | Enterprise(9) setup per host |
| 6.3 | Services & Service Map |
| 6.2 | User Sessions fix |
| 6.1 | Session states & HR API fix |
| 6.0 | Sessions, access logs, poller & reports |
| 5.9 | SQL catalog |
| 5.8 | Tickets, groups & customers |
| 5.7 | App UI codebase |
| 5.6 | Performance |
| 5.5 | Platform charts |
| 5.4 | User roles & host fix |
| 5.3 | Users & Access |
| 5.2 | API sign-in & Weather test |
| 5.1 | Proxy servers |
| 5.0 | Interface discovery |
| 4.9 | Infrastructure |
| 4.8 | Back-end interfaces |
| 4.7 | Public welcome |
| 4.6 | Enterprise(9) page |
| 4.5 | Log discovery |
| 4.4 | Enterprise(9) Logs |
| 4.3 | Safe Audit host |
| 4.2 | Login |
| 4.1 | Apps |
| 4.0 | Host Exceptions |
| 3 | AutoWalk of Swagger / Bug Fixes |
| 2 | Home Page |
| 1 | Base |

## Pages
| Page | File | API |
|---|---|---|
| Sign in | login.html | /api/Auth/signin, then /api/Auth/login (audit / audit is a built-in test login) |
| Welcome (public) | index.html | quick counts (apps, hosts, endpoints, audits; exceptions are on the Dashboard since 6.0); top bar: /api/Userhelp, /api/usernotices, /api/userprofile |
| Dashboard | apidashboard.html | all of them |
| Apps | apiapps.html | /api/Application, /api/ApplicationApi |
| Hosts | apihosts.html | /api/Apihosts |
| Endpoints | apiendpoints.html | /api/Apiaudit |
| Interfaces | apiinterfaces.html | /api/ApiInterfacesAudit (+ each host's own SystemConsole) |
| Run Audit | apiaudit.html | reads Apiaudit + Apihosts, writes ApiAuditResults; Walk Swagger also writes ApiInterfacesAudit |
| Exceptions | apiexceptions.html | /api/AuditExceptions (+ /active, /audit/{id}) and /api/ApiHostException |
| History | apihistory.html | /api/ApiAuditResults |
| Infra Audit | infraaudit.html | reads all of the infrastructure routes below + /api/Application (no writes) |
| Web Servers | webservers.html | /api/webservers (+ /api/webfarmservers) |
| Web Farms | webfarms.html | /api/webfarms, /api/webfarmservers |
| Database Servers | dbservers.html | /api/databaseservers |
| Databases | databases.html | /api/databases |
| Users | users.html | /api/Users, /api/Auth/signup, /api/apiaccesspermissions, /api/siteaccesspermissions (+ Application, ApplicationApi, Apihosts, sites) |
| Sites | sites.html | /api/sites (+ /api/siteaccesspermissions) |
| Re-audit problems (Ops) | reaudit.html | reads /api/Apiaudit, /api/Apihosts, /api/WorkerTroubleTickets; writes Apiaudit stamps and tickets |
| Surveillance Tickets (Ops) | tickets.html | /api/WorkerTroubleTickets |
| Customer Troubles (Ops) | customer.html | /api/Userhelp (read-only) |
| Poller (Ops) | poller.html | reads /api/Apihosts, /api/Apiaudit; pings IPs (DNS over HTTPS), never calls the APIs |
| Database Tables (Infrastructure) | databasetables.html | <source>/api/databasetables (GET; PUT /{id} saves Schema Area); its Fields panel: <source>/api/databasefields (GET /table/{tableName}, POST, PUT /{id}, DELETE /{id}, GET /discovery) |
| Reports | reports.html → report01.html, report02.html | report01: /api/Apiaudit, /api/Apihosts, /api/AuditExceptions, /api/ApiHostException (read-only) |
| User Sessions (Ops) | usersessions.html | /api/Usersession (GET; PUT to expire), /api/Users (role check) |
| Groups (Users & Access) | groups.html | /api/Usergroups (+ /api/Users for members) |
| Customers (Users & Access) | customers.html | /api/Customers or /api/Customer (+ /api/Userhelp for ticket counts) |
| SQL Catalog (Infrastructure) | sqlservers.html → sqltables.html?server= → sqlfields.html?table= | /api/AuditSqlServers, /api/AuditSqlTables, /api/AuditSqlFields |
| Weather (Utilities) | weather.html | /api/weather (+ /api/Usersession for Recent sessions) |
| Enterprise(9) | enterprise9.html | each application's own API: GET /api/<Log> for every Enterprise(9) log |

## Files
- `auth.js`: sign-in guard, loaded first in every page's `<head>`.
- `config.js`: API base URL, routes, timeouts, copyright line. (Replaces config.json, which broke when pages were opened from disk.)
- `schema.js`: field lists and types for each resource, plus `summarizeAudit` / `buildAuditResultPayload`, the summary record Run Audit saves.
  All four resources match the real API models.
- `app.js`: shared layout, API client, tables and add/edit dialog.
- `infra.js`: Infrastructure checks (Infra Audit rules), loading, and the Findings badges.
- `sqlcatalog.js`: SQL catalog loading, findings, the import query and paste parser.
- `db/AuditSql-5.9.sql`: the three catalog tables.
- `groups.js`: user groups (Usergroups and the User groupid1–5 slots), findings and the Groups / Members dialogs.
- `tickets.js`: trouble tickets, problem classification (hard down / 5xx / very slow) and the endpoint probe used by Re-audit.
- `swagger-import.js`: the "Add API from Swagger" / "Sync Swagger" wizard.
- `interfaces.js`: back-end interface discovery (SystemConsole), matching, and endpoint suggestions.
- `db/ApiInterfacesAudit-5.0.sql`: the database change for build 5.0.
- `db/Applications-UI-5.7.sql`: the Applications columns for build 5.7, with the matching model properties.
- `site.css`: styles.
- `poller.html`, `reports.html`, `report01.html`, `usersessions.html`: build 6.0 pages.
- `js/cockyciphers.js`: encrypt3/decrypt3, encrypt7/decrypt7, encrypt96/decrypt96 (byte-shift Caesar, JSON out).
- `api/HRController-6.1.cs`, `api/Program-HR-auth-6.1.cs`: hardened HR utility API (build 6.1).
- `api/CustomerEndpoints-6.2.cs`: the Customer endpoints for build 6.2 (GET includes Portfolios, PUT saves Tenantid / CreatedAt, DELETE refuses a customer with portfolios).
- `api/UsersessionEndpoints-6.2.cs`: the Usersession endpoints for build 6.2 (PUT saves Sessionend / Sessioncomplete even when Sessiondescription is null; POST works for a user's first session).

## Build 6.7: database fields and the storage fix

**Database Fields** (`DatabaseField`: Id, DatabaseTableId, TableName, SchemaName, FieldName, DataType, ClrType, IsNullable, MaxLength,
IsPrimaryKey, CreatedDate, LastAuditDate, IsActive) are managed inside **Database Tables**, not from the menu, on the same source API:
- a **Fields** column: the field records for each table (from `GET /api/databasefields`), ⚠ when it differs from the discovered column count;
- **Fields** on a row (or click the table) opens a panel under the grid with that table's fields (`GET /api/databasefields/table/{tableName}`,
  matched on `DatabaseTableId`, else schema + table name): filter, sort, **+ Add field** (pre-filled with the table's id, schema and name),
  **Edit** (form or JSON), **Delete**; the header shows the field count, the discovered column count and the primary key;
- **Discover fields** (toolbar) / **Discover** (panel) run `GET /api/databasefields/discovery` and refresh the counts.
`databasetables.html?table=<id>` opens a table's fields directly.

**Storage fix (screen fragments like `{"cipher":"caesar96",...}`).** localStorage belongs to the whole origin, not to one app. Builds 6.1–6.6
kept the session under bare names (`token`, `fullname`, `userid`, …) and `migrateAll()` encrypted *every* plain value on the origin, so another
app served from the same site found its own `fullname` / `token` replaced with caesar96 JSON and printed it. Now every CockyAuditor key is
`cocky.<name>` (`cocky.token`, `cocky.fullname`, `cocky.auditor`, …) and the store never touches anything else. On the first page load,
CockyAuditor's old encrypted bare-name values are moved under the prefix and removed; plain values under those names (another app's) are
left alone; a value encrypted twice is unwrapped. If the other app's sign-in was overwritten, sign in to it again once.

## Build 6.6: sortable grids, schema areas and the token / auth audit

**Sortable grids.** Every grid on a page outside the Ops menu sorts by clicking a column header (again to reverse; Enter / Space
from the keyboard), with Bootstrap Icons arrows. It's one shared piece of `app.js` (`Cocky.sortableGrids`), switched on by
`Cocky.layout()` for non-Ops pages, so grids drawn later (search, refresh, dialogs) pick it up too and keep their sort.
Numbers (`1,234`, `12 ms`, `45%`), dates and text are detected per column; blanks (`—`, `N/A`) always sort last; a cell's
`data-sort` wins over its text; detail rows stay under their row, group heading rows keep their groups, totals (`tfoot`,
`.grid-total`) stay at the bottom. Opt out with `data-nosort` on a table or header, or `sortableGrids: false` in config.js.
Ops pages are unchanged (User Sessions and Poller have their own sorting).

**Schema Area (Database Tables).** `DatabaseTable.SchemaArea` (what the table is used for: User Functions, Audit Functions, ...)
shows as a column with an area filter. **Edit** on a row, or tick several and **Set area for selected**. The dialog suggests
`config.js → schemaAreas` plus any area already in use, and any new name can be typed (the case is matched to an existing
area). Saving sends `PUT <source>/api/databasetables/{id}` and reads the row back to confirm it was kept.
Backend: `api/DatabaseTableEndpoints-6.6.cs` (GET /{id}, PUT /{id} that only changes SchemaArea) and
`db/DatabaseTable-SchemaArea-6.6.sql`. The host-side discovery must not overwrite SchemaArea when it refreshes a table.

**Report 02: database tables by schema area** (`report02.html?source=<host url>`). For one API or **All APIs**: total tables,
schema areas used, % assigned, unassigned; a table of areas with the number and % of tables; for All APIs a matrix of API × area;
and the tables in each area (what each table is used for). Inactive tables are left out unless ticked. Print and CSV.

**Token / auth audit (Run Audit).** The aim is that every endpoint checks a token or some login before it does anything.
With **Token / auth check** ticked, each GET route is called:

| Call | Must get |
|---|---|
| no credentials | refused: 401 / 403 or a redirect to sign-in |
| invalid credentials (bad token, bad username / password, bad API key) | refused |
| valid credentials (when available) | through (2xx; 403 = authenticated but not permitted, noted) |

The credential method is per host (**Credentials…**) with a per-endpoint override (*credentials* link in the row), held in memory only:
**Auto** (a `{token}` route parameter is a path token, e.g. `/api/weather/{token}`; `/wp-json/` or an Auth Type of Basic is
Basic; otherwise your signed-in token as `Authorization: Bearer`), **Bearer**, **Token in the path**, **Username + password**
(Basic; WordPress application passwords), **API key header**, **None**. A 2xx without or with invalid credentials = **Open** /
**Accepts invalid credentials**; a 400 / 415 / 422 / 5xx without them = reached the code without auth = Open. A 404 / 405 or a
CORS block = **Not verified**. Routes in `authCheck.publicRoutes` (sign-in, health, swagger) are **Public by design**.
POST / PUT / PATCH / DELETE are only sent when **Also probe writes** is ticked: no body, never with valid credentials (an endpoint
that doesn't check auth may still run its code, so it's off by default; unprobed writes are *Not verified*).

For an endpoint that isn't enforced, CORS and IP exposure are measured as well:
- **CORS**: a wildcard (`*`) is detected because browsers refuse `*` for a credentialed request; a named origin, or CORS blocking
  this page, counts as restricted. (A server that echoes back every origin looks restricted from here.)
- **IP**: the host name is resolved (DNS over HTTPS, `poller.dns`); a private address counts as restricted, otherwise the
  **Allowed Ranges** recorded on the endpoint or its host (anything but `*` / `0.0.0.0/0`). Access restrictions can't be tested from
  inside the allowed network, so **record Allowed Ranges on each host** (e.g. your UI's outbound IPs) for the practical view.

| View | Compliant when |
|---|---|
| **Strict** | the endpoint itself refuses calls without a valid token / login (or is public by design), as if the whole world could reach it |
| **Practical** | Strict, **or** CORS restricted **and** IP restricted (the world can't reach it; CORS alone doesn't stop curl or scripts) |

Each row shows `Token ✓` / `Open` / `Auth ?` plus **S** and **P** badges; the toolbar shows Strict x/y and Practical x/y. An open
endpoint that is practically compliant is a warning, not a failure. Saving the run (with *Also stamp*) writes `AuthEnforced`,
`CorsRestricted`, `IpRestricted`, `AuthCheckResult`, `AuthCheckDate` on each endpoint (`db/ApiauditAuthCheck-6.6.sql` + model
properties) and a summary line in History. **Report 01** has a new check, **Token or auth enforced**, and a **Strict / Practical**
selector (`report01.html?view=practical`); endpoints never checked fail it until an audit stamps them. Settings: `config.js → authCheck`.

## js/cockyciphers.js (build 6.1)
Caesar shifts over byte values instead of the A–Z alphabet, in pairs: `encrypt3` / `decrypt3`, `encrypt7` / `decrypt7`,
`encrypt96` / `decrypt96` (each `decryptN` refuses JSON made with another shift), plus `encrypt(text, n)` / `decrypt(json)` for any shift. The text is UTF-8 encoded (ASCII characters keep their ASCII code), each byte is shifted and wrapped at 256,
and the functions return a JSON string:

```json
{"cipher":"caesar3","version":"1.0","shift":3,"range":256,"encoding":"base64","length":5,"bytes":5,"createdAt":"2026-10-02T23:00:00.000Z","data":"S2hvb3I="}
```

The decrypt functions accept that string or the object (`{ asObject: true }` returns the object) and checks the byte count. Works for long strings (20 MB
round-trips in about 0.4 s) and any Unicode. Browser: `<script src="js/cockyciphers.js">` → `window.CockyCiphers`; Node: `require("./js/cockyciphers.js")`.
A Caesar shift only hides text from a casual look and anyone can reverse it, so don't use it for passwords, tokens or personal data.

## localStorage is stored with caesar96 (build 6.1)
Everything CockyAuditor keeps in localStorage goes through `CockyCiphers.store` (in `js/cockyciphers.js`, loaded before `auth.js` on every page):
the **keys stay readable** and, since build 6.7, all start with `cocky.` (`cocky.userid`, `cocky.token`, `cocky.role`, `cocky.username`, `cocky.email`, `cocky.fullname`, `cocky.testlogin`, `cocky.sessionid`, `cocky.auditor`, `cocky.poller`,
`cocky.sessions.cols`) and **every value is stored as `encrypt96` JSON**, e.g.
`userid` → `{"cipher":"caesar96","version":"1.0","shift":96,…,"data":"kZCQkJA="}`.

- Pages use `CockyCiphers.store.get / set / remove / getJSON / setJSON`; nothing reads or writes `localStorage` directly.
- Values saved in plain text by an older build are encrypted the first time a page loads (`store.migrateAll()` in `auth.js`), so nobody is signed out.
- A value that's been altered and no longer decrypts reads as missing, so a damaged token sends you to sign-in.
- If a page doesn't load `js/cockyciphers.js` before `auth.js`, nothing is read or written and the page asks for sign-in (the console says why).
- sessionStorage (the Weather page's token, the access-log warning) is not covered.
- This keeps values from being read at a glance in the browser's storage panel; anyone with `js/cockyciphers.js` can reverse it.

## Build 6.1: session states and the HR API fix

**Session states** follow the sign-in flow: username, BCrypt password, Usersession created, 6-digit challenge stored, code accepted ->
`Sessioncomplete = 1`, `Acknowledged = 1`, session cookie issued. So **`Sessioncomplete = 1` means signed in**, not closed.

| State | Rule | Shown as |
|---|---|---|
| Active | `Sessionend` blank, `Sessioncomplete` = 1 and `Acknowledged` = 1 | green **Active** |
| Pending 2FA | `Sessionend` blank, code not accepted yet | yellow **Pending 2FA** (filter: Pending 2FA) |
| Closed | `Sessionend` set (sign-out or expired here) | grey **Closed** |

Expiring still sets `Sessionend` = now and `Sessioncomplete` = 1; `Sessionend` is what closes it. *Users signed in* counts Active sessions only.
Sign-out should also set `Sessionend` (the Users logout route currently only "marks the session complete", which under this flow isn't a close).

**HR utility API** (`api/HRController-6.1.cs`, `api/Program-HR-auth-6.1.cs`), for the roles in the design (Role 1 User / SuperUser / Admin,
Role 2 Auditor / Accountant, Role 3 SwaggerAdmin):

| Route | Who | Returns |
|---|---|---|
| `GET /api/hr`, `/api/hr/{username}` | HRRead: Auditor, Accountant, Admin, SuperUser | EmployeeId, Username, SSN as `***-**-1234`; Salary and TotalStockOptions only for HRFull (Accountant, SuperUser), else null |
| `GET /api/hr/whoami` | HRAdmin: Admin, SuperUser | the SQL login the API connects as |

Every route also needs the caller's token (Bearer header, or the session cookie named in `SessionCookieName`) to match a Usersession that is
not ended and has passed 2FA, so a session expired on User Sessions loses HR access at once. Every call goes to ApiLogger. The JWT needs a
`role2` claim (see the end of `Program-HR-auth-6.1.cs`).

## Build 6.0: access logging
Every sign-in writes three records through the API (`config.js` → `accessLog`):

| Event | Logs | Note written |
|---|---|---|
| Sign-in (API user or the audit / audit test login) | `POST /api/Userlog`, `/api/SysLog`, `/api/Superuserlog` | Userlog: *Signed in to CockyAuditor build 6.0*; Syslog: *Auditor access: auditor (user #10000, role auditor, session #…) signed in to CockyAuditor from <origin>*; Superuserlog: *CockyAuditor tool access granted to …* |
| Failed sign-in (wrong user name or password) | Syslog | *Failed CockyAuditor sign-in for "<name>" from <origin>* (never the password) |
| Sign-out | Userlog | *Signed out of CockyAuditor* (sent with keepalive as the page leaves) |
| Users → Profile | Syslog | *<auditor> reviewed the profile of user #5 (jdoe)* |

- **The record sent** is `accessLog.logs.<name>.body`, with `{placeholders}` filled in (`event`, `note`, `now`, `userid`, `useridtext`, `username`,
  `fullname`, `email`, `role`, `sessionid`, `testlogin`, `app`, `build`, `origin`, `page`, `agent`). The defaults carry several common names
  (`description` / `message`, `action`, `logdate` / `createddate`, …); ASP.NET ignores properties a model doesn't have. **Replace them with your
  Userlog / Syslog / Superuserlog model's column names** to be sure each note lands in the right column.
- If a log answers 400 naming fields, those fields are retried as text instead of a number (or the reverse), and a required blank field gets
  `N/A` / 0, then it's sent once more. Each log waits at most `timeoutMs` (5 s).
- **A log that can't be written never blocks the sign-in**; the next page shows a warning naming the log and the reason.
- These are client-side writes: they show who used the tool, but a determined user could skip them. For tamper-proof records, write the same
  rows inside `/api/Auth/signin` on the API.

## Build 6.0: Users → Profile
Each user row has **Profile**, a read-only modal with the **entire User record**: grouped into Identity, Roles, Groups (each slot with the group's
name, or *no such group*), Applications & access, Address, Status & dates, Security and Other fields, with the user's Findings on top.
Password, hash, salt, reset token, 2FA key and token fields show only *set, hidden*. Tabs: **Profile** (the user's `/api/userprofile` record, matched
on user id, then email / user name), **Sessions** (their Usersessions, newest first, with time open and a link to User Sessions) and **JSON** (secrets
replaced by `(hidden)`). **Edit user** and **Access** open the existing dialogs. `users.html?profile=<id>` opens it directly. Viewing writes a Syslog line.

## Build 6.0: Poller (Ops)
`poller.html` opens a **wizard**: 1) tick the API hosts, 2) tick their endpoints (active ones pre-ticked, with the host : port each will be pinged
at), 3) interval (5–60 s), stop after (continuous or N rounds) and the no-reply limit. `poller.html?host=<id>` pre-selects a host; your last choice is remembered.

**What a ping is.** Browsers can't send ICMP (and Azure App Service drops ICMP anyway, so a true ping to `*.azurewebsites.net` would always fail).
So the poller never calls the API: it resolves the endpoint URL's host name to an **IP** with DNS over HTTPS (`poller.dns`: Google, then Cloudflare)
and times a bare connection to `https://<ip>:<port>/` (no-cors, no credentials, no path, nothing read). An answer of any kind, including a refused
connection or a certificate error, means the host's network stack replied: **up**, with its time. No reply within the limit: **down**.
Endpoints sharing an IP and port are pinged once. For real ICMP, host a small ping service and set `poller.pingService`
(e.g. `https://ops.example.com/api/ping?ip={ip}` answering `{ "ok": true, "ms": 12 }`).

Grid (every column sorts): status (down since), API host, host name (and CNAME), IP, port, endpoints, last / avg / min / max ms, loss %, a strip of
the last 30 pings, last checked; click a row for the endpoints and the exact address pinged. **Ping now**, Start / Stop, Down-only filter, **Download CSV**.
Caveats shown on the page: private IPs (the browser may block them, so check from inside the network), ports browsers refuse (e.g. 22, 25), names that don't resolve.

## Build 6.0: Reports
**Reports** (sidebar section) opens `reports.html`, the list of system reports from `config.js` → `reports` (file, title, description). Add a
report by adding a `reportNN.html` page and a line there.

**Report 01: API compliance by endpoint** (`report01.html`). Scored from the stored endpoint records; no API is called.

| Check (`compliance.checks`) | Passes when |
|---|---|
| HTTPS (this is "secure") | the endpoint's URL (Run Audit's rules) is `https://` and the last audit didn't stamp `endpointSecure = false` |
| Authentication enabled | `hasAuthEnabled` |
| Auth type recorded | authentication enabled and `authType` filled |
| Audited recently | `lastAuditDate` within `compliance.auditMaxDays` (90) |
| Security contact | `securityEmail` or `techContactEmail` |
| Classification recorded | `securityClassification` |

Each failed check is one **security warning**; an endpoint's **compliance % = checks passed ÷ checks run**. Totals at the top: **Total endpoints**,
**Total secure endpoints**, **Total security warnings**, **Compliance %** (all checks passed ÷ all checks run), plus a pass rate per check. Then one
section per API host with its own subtotal (or *Lowest compliance first*). Inactive endpoints and those covered by a live endpoint or host exception
are left out (tick to include). **Print** uses a print layout (no sidebar or buttons); **Download CSV** gives one row per endpoint and a total row.
`report01.html?host=<id>` opens one host.

## Build 6.0: Exceptions on the Dashboard
The Welcome page no longer shows exception counts or the Exceptions feature card. The Dashboard keeps its endpoint / host exception tiles and the
**Exceptions** card (live host and endpoint exceptions), which now also lists **expired exceptions still marked active**.

## Build 6.0: User sessions (Ops)

**User Sessions** (`usersessions.html`, Ops menu) is the operations view of who is signed in, from `/api/Usersession`
(`Enterprise.Models.Usersession`). A session is **open** while `Sessionend` is blank.

- **Grid** (Bootstrap, `table-striped table-bordered`): click any column header to sort (again to reverse); every column sorts,
  including the ones turned on from **Columns** (all Usersession fields; your choice is remembered in this browser).
  Default columns: Id, User id, User name, Full name, Email, Session start, **Time open**, Session end, Status, Description.
  Search, Open / Closed / All, age filter (over 1 day / under / no start time), 25 / 50 / 100 per page. Click a session for every field.
  `usersessions.html?state=all|closed&age=stale&q=<text>` opens pre-filtered.
- **Time open** = `Sessionstart` to now, updated every 30 s; green, yellow past 8 h, red past 1 day (stale rows are shaded). For a
  closed session it's start to end. `Sessionstart` / `Sessionend` are strings: ISO (.NET `"o"`, 7 fraction digits) is read as UTC;
  a value the page can't read is shown in red and the session counts as "no start time".
- **Tiles:** open sessions, distinct users signed in, open over 1 day, closed today (click to filter).
- **Secrets are never shown:** `Token`, `GoogleToken`, `FacebookToken`, `MicrosoftToken`, `Targetcipher`, `Twofactorkey`,
  `Twofactorprovidertoken`, `Twofactorproviderauthstring` show only *set* / — (sortable by that), are left out of search, and the
  detail view gives only their length.

**Expiring sessions** (auditors and admins: `userSessions.expireRoles` = auditor, admin, superuser, matched against the signed-in
role and the user's Role 1 / 2 / 3 on `/api/Users`, so a Role 2 *auditor* counts):
- When the page opens and there are open sessions older than `userSessions.expireAfterHours` (24), it asks
  **"Expire all user sessions over 1 day old?"** with the list (user, start, time open). **No** leaves them; the
  **Expire sessions over 1 day** button asks again later. Each open row also has **Close**.
- **Records are never deleted.** Each is updated with `PUT /api/Usersession/{id}`: the record as read, with `Sessionend` = now
  (ISO UTC) and `Sessioncomplete` = 1; `Keyassignments` is sent as `[]`. Four at a time, with a progress bar.
- Your own session (matched on session id or token) is never expired from here.
- Afterwards the page **re-reads the table** and reports closed / failed / *accepted but not saved*.

**API (build 6.2):** the Usersession PUT used to skip the whole update when `Sessiondescription` was null (still answering 202).
The page now always sends a `Sessiondescription` ("Session closed" when blank), so it works with that PUT too; `api/UsersessionEndpoints-6.2.cs`
removes the check. Note the weather API's token check still doesn't look at `Sessionend`, so a closed session's token keeps working there
until that check is changed too.

## Build 5.9: SQL catalog

**Database** (`db/AuditSql-5.9.sql`): `AuditSqlServers`, `AuditSqlTables`, `AuditSqlFields`, plus the permission tables
(`AuditPrincipals`, `AuditServerPermissions`, `AuditTablePermissions`, `AuditFieldPermissions`) used by the next build.
**API:** `AuditSqlController` (`app.MapAuditSqlEndpoints()`), all under `/api/AuditSql`: `/Servers`, `/Tables`, `/Fields`,
`/Principals`, `/ServerPermissions`, `/TablePermissions`, `/FieldPermissions` (routes in `config.js` → `endpoints.sql*`).
Its DELETEs don't check for child rows, so the pages delete children first (a table's columns) or refuse (a server with tables);
once permission rows exist, deleting a table or column they point at will fail until those grants are removed.

**Pages** (Infrastructure → SQL Catalog): SQL servers → **Tables** → **Fields**, each with add / edit / delete and a breadcrumb.
- **SQL servers:** tiles (databases, tables, columns, PHI / PII columns, sensitive but not encrypted), owner and DBA, table and
  column counts, PHI / PII tables, Findings. A server with tables can't be deleted until its tables are.
- **Tables:** `sqltables.html?server=<id>` (or all), filter (with findings / holding PHI / holding PII), column count with
  *no PK*, PHI / PII flags with column counts, rows. Deleting a table deletes its catalog columns first (restored if the delete fails).
- **Fields:** `sqlfields.html?table=<id>`, type with length, PK / FK, nullable, PHI / PII, encrypted, filter (PHI / PII, not
  encrypted, looks sensitive), **Flag suggested** (flags columns whose names look sensitive, after you confirm, and their tables).

**Import from SQL** (button on each server): copy the query shown (SQL Server / Azure SQL: every user table and column with PK,
FK, nullable, Always Encrypted and row count), run it in that database, paste the result (SSMS *Copy with Headers*, CSV or
JSON). **Preview** shows new tables, new columns and what's already catalogued, plus PHI / PII suggested from column names
(`sqlCatalog.phi` / `sqlCatalog.pii` regexes) to tick or untick. **Import** adds only what's missing (existing rows are never
changed) and sets each table's Contains PHI / PII from its flagged columns. Other database types: paste any result with the same
column names (SchemaName, TableName, ColumnName, DataType, …; INFORMATION_SCHEMA names also work).

**Protection fields:** servers — size, backup policy, TDE, Defender for SQL, geo-replication (a *Protection* column); tables —
size, statistics date, row-level security, dynamic data masking, classification status (*Controls*); columns — sensitivity
label and masking rule. The import query (SQL Server 2016+ / Azure SQL) reads size, statistics, RLS and masking too, and on a
re-import **Refresh measured facts** updates row count, size, statistics, RLS, masking, encryption, keys and type on what's
already catalogued; PHI / PII flags, descriptions, labels and classification are never changed by an import.

**Findings:** server — holds PHI without TDE (**fail**; PII: warn), Defender off, no backup policy, production without
geo-replication (note); table — unencrypted PHI / PII with neither RLS nor masking (warn), masking on but no rule recorded,
statistics older than `sqlCatalog.staleStatsDays`, sensitive table not Classified / Approved; column — sensitive without a
sensitivity label, PII masked but not encrypted (note instead of warn);
column — PHI not encrypted (**fail**), PII not encrypted (**warn**), name looks sensitive but isn't flagged;
table — no primary key, unencrypted PHI / PII columns, PHI / PII columns without the table flag (and flags with no such column);
server — no owner, no DBA, unknown database type, version past end of support (the `infraAudit.endOfSupport.database` rules,
so MSSQL 2016 fails), and a roll-up of its tables. Levels for unencrypted columns: `sqlCatalog.unencryptedPhi / unencryptedPii`.

This catalog is also the inventory the SQL **permissions** audit (planned next) will check grants against.

## Build 5.8: groups, customers, Dashboard tickets

**Groups** (Users & Access, `groups.html`; `Usergroup`: Id, Groupid, Groupdescription, Groupownerid, Groupcompanyid). A user's groups
are their `groupid1`–`groupid5` (up to five), each holding the group's **Groupid** code (`config.js` → `groups.userSlots`).
- Groups page: add / edit / delete groups (owner picked from Users), **Members** to tick who's in it (users already in five
  other groups can't be added), member names and counts, Findings (no owner, owner missing, no code, duplicate code, no members,
  members from another company). Deleting a group takes it off its members first, and puts it back if the delete fails.
- Users page: **Groups** column, group filter (`users.html?group=<id>` or `none`), **Groups** button per user. Findings: a slot
  naming a group that doesn't exist (the Groups dialog can clear it), a slot holding the numeric id or description instead of
  the code (fixed on next save), and membership of another company's group (`User.companyid` vs `Groupcompanyid`).
- Saving sends the registration fields and roles with the five slots, like Edit user; passwords are never sent.

**Customers** (Users & Access, `customers.html`; `Customer`: Id, Tenantid, UserId, FullName, CreatedAt, Portfolios): add, edit,
delete; CreatedAt defaults to now; a customer with portfolios can't be deleted here. *Customer tickets* shows open / total from
Userhelp, matched on the ticket's user id, linking to Customer Troubles filtered to that user. The route is `/api/Customers`,
falling back to `/api/Customer`. **Customer Troubles** shows each ticket's customer name and tenant from the same match, with a
tenant filter.

**Dashboard → Tickets:** two stacked bar charts, *Surveillance tickets by type* (Hard down / Server error (5xx) / Very slow from
the ticket's first description line, Hard down for IsHardDown, otherwise Other) and *Customer tickets by type* (the category /
type field), each bar split into Open / In progress / Closed. Hover a segment for its count; click a bar to open the tickets.

## Build 5.8: Trouble tickets, Re-audit, OS & hosting columns

**API:** `WorkerTroubleTickets` with `app.MapWorkerTroubleTicketEndpoints()` (route in `config.js` → `endpoints.troubletickets`).
The auditor uses GET, POST, PUT and DELETE; "open" is worked out in the browser (any status not in `tickets.closedStatuses`), so
*In Progress* tickets count as open too.

**What counts as a problem** (`config.js` → `tickets.problems`, thresholds from `performance`):

| Kind | Rule | Severity | Ticketed by default | IsHardDown |
|---|---|---|---|---|
| Hard down | no response (timeout, refused, DNS, TLS), or stamped LastStatusCode 0 | Critical | yes | 1 |
| Server error | HTTP 500 or higher | High | yes | 0 |
| Very slow | answered, slower than `failMs` (5 s) | Medium | no | 0 |

**A ticket per endpoint.** ApiName = the host's name, Endpoint = `GET /path`, Environment = endpoint's, else host's, else `Unknown`,
TicketNumber = `CA-<yyyyMMdd-HHmmss>-<endpoint id>`, ReportedBy = the signed-in user. If that endpoint already has an open ticket
(same ApiName + Endpoint), its IncidentCount goes up instead; a hard-down repeat also sets IsHardDown and raises the severity.

- **Run Audit:** after a run, a *Problems in this run* card lists hard-down, 5xx and very slow endpoints with any open ticket;
  tick and **Create trouble tickets** (you confirm the list first).
- **Re-audit problems** (Ops): the endpoints whose last saved result was hard down / 5xx / very slow, plus every endpoint with an
  open ticket. **Re-audit selected** re-checks just those and shows *Recovered*, *Still failing*, *Better* or *Worse*. Then:
  **Save results to endpoints** (LastStatusCode / LastResponseMs / LastAuditDate; no History record), **Open / update tickets**
  for the ones still failing, **Resolve recovered tickets** (notes say when and how it answered). Filters by problem type, ticket
  and host; `reaudit.html?ticket=<id>` opens on that ticket's endpoint and re-checks it straight away. Open tickets that don't
  match a registered endpoint are listed.
- **New ticket fields** (`ImpactedUsers`, `BusinessUnit`, `RootCause`, `EvidenceUrl`, `AuditorNotes`, `ApplicationOwner`), in the
  form under *Impact & ownership*, *Evidence & notes* and *Assignment & resolution*. On a new ticket the auditor fills
  **Application owner** and **Business unit** from the applications linked to the endpoint's host (ApplicationApi; else the
  endpoint's / host's tech contact and business unit), **Evidence URL** with the URL it tested, and **Auditor notes** with the
  check's warnings and failures. **Impacted users** and **Root cause** are left for people; **Resolve** asks for both notes and root
  cause. A repeat on an open ticket adds a line to Auditor notes. The grid's *Owner* column shows owner and business unit, and
  *Incidents* shows impacted users.
  **API:** the PUT in `WorkerTroubleTicketController` copies fields one by one, so add the six new ones there or edits to them won't save.
- **Surveillance Tickets** (Ops; the Operations Surveillance / Tier 2 queue, `tickets.html`): tiles (open, hard down, critical, resolved), status filter (`tickets.html?status=all|harddown|Resolved`),
  add / edit / delete, **Resolve** (asks for notes) or **Reopen**, and **Re-audit** per ticket.

**Customer Troubles** (Ops, `customer.html`): the customer tickets in `/api/Userhelp` (`endpoints.customertroubles`), read-only,
as a Bootstrap grid: newest first, click a column to sort, search, status filter (with Open / Closed), priority filter, 25 / 50 / 100
per page with pagination, tiles (total, open, high priority open, last 24 hours), and every field when you click a ticket. Columns are
picked from the records (id or ticket number, date, subject with the message under it, customer with email, category, status,
priority, assignee), so the page works whatever Userhelp's field names are; to fix the columns list them in `customerTroubles.columns`.
A ticket counts as closed when its status matches `customerTroubles.closedMatch` (closed, resolved, done, …).

**Apps grid:** *Codebase* (badge + UI vendor) and *App OS* (the UI operating system for its codebase, with `API: OS · vendor`
underneath). **Hosts grid:** *Operating system* and *Hosted by*, from `OS_API` and `APIVendor` on the applications linked to the
host. Fallbacks: the host's own OS type ("from host"), and a vendor guessed from the URL ("from URL", rules in
`appUi.hostingFromUrl`, e.g. `azurewebsites.net` → Microsoft Azure). A **differs** badge shows when the linked apps disagree.
API vendor suggestions in the app form are hosting vendors (`appUi.apiVendors`).

## Build 5.7: App UI codebase

**Database** (`db/Applications-UI-5.7.sql`): seven nullable `varchar(255)` columns on `Applications`: `UI_Codebase`, `APIVendor`,
`UI_HTMLVendor`, `UI_REACTVendor`, `OS_UIHTML`, `OS_UIREACT`, `OS_API`. Add the same properties to `Enterprise.Models.Application`
and redeploy the API. CockyAuditor matches field names without case or underscores, so `uI_Codebase` (.NET's default JSON name),
`UI_Codebase` and `uiCodebase` all work.

**UI_Codebase is the key field.** It decides which vendor / OS fields apply (`config.js` → `appUi.codebases`):

| Codebase | UI fields it needs | Rule |
|---|---|---|
| mobile | HTML UI or React UI | at least one set complete |
| html | HTML UI (`UI_HTMLVendor`, `OS_UIHTML`) | both filled |
| ionic | HTML UI or React UI | at least one set complete |
| react | React UI (`UI_REACTVendor`, `OS_UIREACT`) | both filled |

Every app also has the API's vendor and OS (`APIVendor`, `OS_API`).

- **Add / Edit application:** a **UI & platform** section. Codebase is a dropdown (a stored value that isn't in the list is kept
  and marked); vendors and operating systems are free text with suggestions (`appUi.vendors`, `appUi.operatingSystems`). Fields the
  chosen codebase doesn't use are dimmed with a "Not used by … apps" note, but can still be edited and are still saved.
- **Apps grid:** a **UI & platform** column (codebase badge, then the UI and API vendor · OS lines) and a codebase filter with counts
  (`apiapps.html?ui=react`, `?ui=unset`, `?ui=other`).
- **Infra Audit / Findings:** warns when the codebase isn't set or isn't in the list, or when the vendor or OS its codebase needs is
  missing; notes UI details recorded for a UI the codebase doesn't use, and a missing API vendor / OS (`appUi.missingApiLevel`,
  "info" by default). The three OS fields are checked against the end-of-support OS rules (e.g. Windows Server 2012 R2 fails).
- **Dashboard:** a fourth pie, **Apps by UI codebase**, including *Not set* and *Not in the list*; click a slice to open those apps.
- Until the API returns `UI_Codebase`, Apps shows a note naming the columns to add, the filter is hidden, the pie says so, and no
  UI findings are raised.

## Build 5.6: Performance (Ops)

**Database:** two nullable columns on `Apiaudit`, returned by `/api/Apiaudit` as `lastResponseMs` / `lastStatusCode`:
```sql
ALTER TABLE Apiaudit ADD LastResponseMs int NULL, LastStatusCode int NULL;
```

**Run Audit** writes them on every audited endpoint when you **Save audit to History** with *Also stamp the audited endpoints* ticked:

| Result of the check | LastResponseMs | LastStatusCode |
|---|---|---|
| Answered, status readable | time in ms | HTTP status |
| Answered, but CORS hides the status | time in ms | null |
| No response / unreachable | null | 0 |
| Not requested (no URL) | null | null |

**Performance page** (`perf.html`, Ops menu):
- **Scope:** the last audit that stamped endpoints (default), any of the last 15, or *Latest time on every endpoint*. `perf.html?audit=9&host=2` opens pre-filtered.
- **Tiles:** endpoints timed, median, 95th percentile, slowest, Warning (2–5 s) and Slow / no reply (over 5 s, or no response).
- **Slowest endpoints:** D3 bar chart of the top 20 (`performance.chartTop`) with dashed 2 s / 5 s lines; click a bar to open it in Run Audit.
- **By host:** median, slowest and slow count per host; click a host to filter.
- **All timed endpoints:** sortable by endpoint, host, time, HTTP status or date.
- Thresholds live in `config.js` → `performance` (`warnMs` 2000, `failMs` 5000); Run Audit's findings use the same values.
- If `/api/Apiaudit` doesn't return the new fields yet, the page says so (recompile and deploy the API).

Times are measured from the auditor's browser (a single GET, including their network and any TLS setup), so compare endpoints and hosts
within a run rather than reading them as exact server times.

## Build 5.5: platform charts (D3)

Charts use **D3 v7** from jsDelivr (`https://cdn.jsdelivr.net/npm/d3@7/dist/d3.min.js`) and the shared `charts.js` (`CockyCharts.donut`,
`CockyCharts.hostPlatform`). Styles are the `.viz-*` rules at the end of `site.css`.

**Dashboard pies** (top of the page): **APIs by platform** (hosts), **Endpoints by platform** (each endpoint counts as its host's platform)
and **Apps by platform** (Mixed when an app's APIs are on more than one platform; Unknown when it has no APIs linked). Hover a slice or a
legend row for the count and share.

How a host's platform is decided (`config.js` → `platforms`):
1. The host's own `frameworkVersion`, `family`, `databaseFramework`, `sourceRepository`, `applicationName`, `osType`, `type` against
   the rules: **DotNet** (.NET, dotnet, ASP.NET, C#, Entity Framework, net8.0, IIS, Kestrel) and **NodeJS** (Node, Express, NestJS,
   Next.js, Fastify, Koa, JavaScript, TypeScript, npm).
2. Otherwise the most common answer from the host's endpoints (same fields).
3. Otherwise, a host with back-end interfaces read from SystemConsole is DotNet (that console is a .NET service).
4. Otherwise **Unknown**; the card lists those hosts. Fill in *Framework version* in the host's Details (e.g. `.NET 8`, `Node 20`).

To chart another platform add a rule, e.g. `{ name: "Python", match: "python|django|flask|fastapi" }`. Colors follow the rule order, so
a platform keeps its color whatever its share.

## Build 5.4: user roles, Ops menu, host fix

**Roles.** Every user has three roles, chosen in **Add user** and **Edit user** (lists in `config.js` → `roles`):

| Set | Stored in | Values |
|---|---|---|
| Role 1 | `Role` | user, guest, admin, superuser (required; new users start as user) |
| Role 2 | `Role2` | hrmanager, auditor, opsmanager, accountant (optional) |
| Role 3 | `Role3` | SwaggerAdmin, none (required; new users start as none) |

- If your `User` model names the fields differently, change `field` (or add to `aliases`) in `config.js` → `roles`. The first name found on
  the records returned by `/api/Users` is used, in any case.
- Signup doesn't take roles, so a new user is created through `/api/Auth/signup` and the roles are then set with `PUT /api/Users/{id}`.
- A stored value not in its list (e.g. the old `registered`) is kept and shown as "(not in list)"; an unset required role shows "(choose)".
- **Users grid:** Roles column (privileged roles in yellow, unknown ones in red) and a role filter (`users.html?role=2:auditor`).
- **Findings:** *warn* required role not set; *info* role value not in its list; *info* privileged role (superuser, SwaggerAdmin).
- The API must have `Role2` and `Role3` columns on `Users`; otherwise the PUT ignores them or fails.

**Ops menu.** Run Audit, Exceptions and Enterprise(9) moved to a new **Ops** section at the bottom of the sidebar.

**Host fix.** Once `Apihost` had navigation collections (`ApplicationApis`, `ApiAccessPermissions`), adding a host failed: the form sent them as
`null`, then `"N/A"`, and ASP.NET rejected both. Nested arrays/objects in a record are no longer form fields; collections are sent as `[]`.
When the API says a field is required and it's a collection, CockyAuditor now retries with `[]` (this also fixes **Add API from Swagger**).

## Users & Access (build 5.3)
Sidebar → **Users & Access**. The rule: **a user gets API access only for the host APIs of the applications they're permitted to use.**

```
User ── ApplicationIds / ApplicationPermissions ──> Application ──< ApplicationApi >── Apihosts
  ├──< ApiAccessPermissions (UserId, ApiId = Apihosts.Id, PermissionLevel, IsEnabled, GrantedBy)
  └──< SiteAccessPermissions (UserId, SiteId, AccessLevel, IsEnabled, ExpirationDate, ReviewedDate/By)
```

**Users** (`users.html`)
- **+ Add User** asks only for the registration fields (first and last name, user name, email, password) and creates the user with
  `POST /api/Auth/signup`, which stores the password hashed; `POST /api/Users` isn't used because it would store it in plain text.
  Then the Access dialog opens. **Edit** changes the same four fields (`PUT /api/Users/{id}`, only those fields are sent, so everything
  else is untouched); users keep the rest of their profile up to date themselves after signing in. User name and email must be unique.
- Password and reset-token fields are never shown or sent. If `GET /api/Users` returns them, the page shows a **Fail** banner.
- **Delete** removes the user's API and site permission rows first (put back if the delete fails). You can't delete yourself.
- Filters: search, application (`users.html?app=<id>`), with findings, no applications. `users.html?user=<id>` opens that user's Access.

**Access** (click a user):
1. **Applications**: tick the apps and pick a level. Saved on the user as `ApplicationIds` + `ApplicationPermissions` (both `List<string>`,
   stored as JSON). By default the lists are parallel (`["1","2"]` + `["Admin","Read"]`); `config.js` → `access.appPermissionFormat: "pairs"`
   writes `["1:Admin","2:Read"]` instead. Both are read either way.
2. **API access**: only the host APIs linked to the ticked applications are offered, with level and enabled. **Grant all** ticks them all.
   Unticking an application removes API access that no other application of the user still covers. Existing rows outside the user's
   applications are shown in red; untick to remove them.
3. **Site access**: level, enabled, expiry date, and **Reviewed now** (sets ReviewedDate / ReviewedBy). New grants are stamped reviewed.

The footer lists every change before **Save access**. The user's applications are saved first; if that fails nothing else is changed.
New rows get `CreatedDate`/`GrantedDate` = now and `GrantedBy` = the signed-in user. Levels are in `config.js` → `access` (`appLevels`,
`apiLevels`, `siteLevels`, default Read / Write / Admin); a value already stored that isn't in the list is kept.

**Findings** (grid column and top of the Access dialog):

| Fail | Warn | Info |
|---|---|---|
| API access to a host not through any of the user's applications; site access past its expiry but still enabled; plain-text password stored | application or host in the permissions doesn't exist; access to an inactive application; site never reviewed or not within `access.reviewDays` (90); no Granted by; duplicate rows for the same host; account status disabled/locked/terminated but still has access | access to an inactive host or site; ApplicationPermissions count differs from ApplicationIds |

**Sites** (`sites.html`): add/edit/delete sites; the **Users** column shows who has access (expired ones flagged). Deleting a site removes its
access rows first.

## Weather test (build 5.2)
**Utilities → Weather** (`weather.html`) exercises the token-protected weather test API (`config.js` → `weather`, default `/api/weather` on `apiBaseUrl`;
the URL box on the page can point it elsewhere). The API checks the token by looking it up in `Usersessions.Token`, and it takes it
**in the path** (`/api/weather/{token}`, `/api/weather/{id}/{token}`), not in an `Authorization` header.
- **Session token**: paste one, or **Recent sessions** lists the newest 10 records in `/api/Usersession` that have a token. The token is kept in
  sessionStorage, so it's gone when the tab closes.
- **Get forecast**: `GET /api/weather/{token}` shown as 5 day cards (°F, °C, summary). A 1001/1002 answer is shown with what it means.
  Dates are `DateOnly` (`2026-09-29`) and are shown as that calendar day, not shifted by time zone.
- **Run all tests**: calls all 8 routes and checks each answers as designed: without a token `400` + `errorCode 1002`; with the token `200`
  (GET: a list of days; POST/PUT/DELETE: `"POST Success"` etc.). **Include invalid-token tests** adds 4 calls with a made-up token that must
  answer `400` + `1001`. Any `200` without a valid token is a **Fail**. PUT/DELETE use id 1; they're stubs and change nothing.
  With no token entered the with-token routes are skipped.
- PUT and DELETE need a CORS preflight, so the API's CORS policy must allow those methods (`AllowAnyMethod()`).

API notes found while testing:
- `TemperatureF` uses its own random number rather than converting `TemperatureC`, so the two disagree; the page flags it (`≠ °F`).
  Fix: pick `var c = Random.Shared.Next(-20, 55);` once and set `TemperatureF = 32 + (int)(c / 0.5556)`.
- The token check doesn't look at the JWT's `exp` or at whether the session has ended, so any token ever saved in Usersessions keeps working.
- A token in the URL ends up in web server and proxy logs; an `Authorization: Bearer` header would keep it out of them.

## Proxy servers (build 5.1)
`WebServers` has two nullable flags, `IsProxy` and `IsProxySlave` (`isProxy` / `isProxySlave` in the JSON). Each web server has one **role**:

| Role | isProxy | isProxySlave | Meaning |
|---|---|---|---|
| Web server (Backend) | 0 | 1 | a regular web server sitting behind a proxy |
| Proxy server | 1 | – | a proxy web server |
| Role not set | 0/null | 0/null | not classified yet; Infra Audit warns |

A server flagged as both counts as a proxy server (and Infra Audit warns).
- **Dashboard** and **Welcome**: the web server count is split into **Web servers** (backend) and **Proxy servers**, each with its fail/warn totals and a link to Web Servers filtered to that role. Servers with no role set show as "+N role not set" under Web servers.
- **Web Servers**: a **Role** column (Backend / Proxy / Role not set), a role filter (`webservers.html?role=slave|proxy|unset`), servers with no role listed first. **+ Add** on a filtered list starts the new server in that role. The edit form has a **Proxy role** section with both checkboxes.
- **Infra Audit** labels web servers by role and adds: *warn* role not set; *warn* a backend web server is public facing (it should only be reached through its proxy); *warn* backend web servers exist but no active proxy server; *warn* flagged as both; *info* a proxy server with no active backend web servers.
- The API has no field saying *which* proxy a backend server sits behind, so they aren't grouped under their proxy. Adding `ProxyWebServerId int NULL` to WebServers would allow that.

## Back-end interfaces (build 5.0)
Each API reports the services/interfaces registered behind it at its own **SystemConsole**
(`config.js` → `interfaceDiscovery.paths`, default `/SystemConsole` then `/api/SystemConsole`), e.g.
`[{ "interfaceName": "IApilogService02", "methods": ["Create02", "GetAll02", ...] }]`. They're stored in `ApiInterfacesAudit`,
one row per interface method, in two steps:

1. **Imported on audit, against the host.** *Walk Swagger & audit host* (Hosts → Audit host, or Run Audit) reads the host's
   SystemConsole with the same login it used for Swagger, right after registering endpoints. Each method is saved with
   `apiHostId` = the host, `endpointName` = the host's name, and **no endpoint** (`apiAuditId` null). It asks first
   (*Save N methods* / *Skip interfaces*), like new endpoints. Methods are matched on host + interface + method (any case), so
   re-audits never duplicate them. Ones no longer reported get `isRegistered = false`; ones that come back get `true` again.
   A host without a SystemConsole, or one CORS blocks, doesn't stop the audit; the banner says why.
   **Import from host** on the Interfaces page does the same on demand, or from pasted JSON.
2. **Associated with endpoints later.** Interfaces → **Associate endpoints** lists a host's methods grouped by interface, each with a
   dropdown of that host's endpoints. Suggestions come from the names: the interface gives the resource
   (`IPaymentService24` → Payment/Payments, `IApilogService02` → Apilog), the method gives the verb and whether it takes an id
   (`GetAll` → GET without `{id}`, `GetById` → GET with, `Create` → POST, `Update` → PUT/PATCH, `Delete` → DELETE); a
   `swaggerOperationId` equal to the method name wins outright. **Fill suggestions** applies them to the blank rows;
   nothing is saved until **Save**. Saving sets `apiAuditId`, plus `httpMethod`, `route` and `controllerName` (the Swagger tag) from the endpoint.

Also: the Interfaces list filters by host and status (not associated / associated / no longer registered); Hosts and Endpoints
have an **Interfaces** button; Run Audit shows each endpoint's back-end methods as an info line, and the saved History note gets
one line with the interface counts; the Dashboard tile shows how many are not associated.

**Rows from the old paste import** (4.8) have no host and are tied to whichever endpoint was picked in that dialog. The Interfaces
page offers **Move to their host**: host from that endpoint, endpoint cleared. Until then they count as the endpoint's host, so
the next audit won't duplicate them.

**Database:** `ApiHostId int NULL` and a **nullable `ApiAuditId`** (`public int? ApiAuditId`), since a method has no endpoint
until it's associated. `db/ApiInterfacesAudit-5.0.sql` does both (it drops and re-adds the ApiAuditId foreign key, which SQL Server requires).
If `ApiAuditId` is still required, imports fail with a message saying so.

## Enterprise(9) logs
**Rule: every endpoint with "log" in its path is part of Enterprise(9).** **Enterprise(9)** (sidebar, or the **Enterprise(9)** button on an
application in Apps, which shows how many log sets that app's linked hosts have registered) shows, for the chosen application, one **accordion section per log category** (click **+** / **−**, or Expand all /
Collapse all). Opening a section loads a grid of its **newest 25** records; click a row for every field (and the raw JSON).
The header of each section shows its route and, once loaded, its record count.
- **Finding the sets:** the page looks through the endpoints registered on the application's hosts. Only the path counts,
  not the host name (`capitoltechnology` contains "log"). Endpoints are grouped by the path segment with "log" in it:
  `/api/Apilog`, `/api/Apilog/{id}` and `POST /api/Apilog` are the **Apilog** set. A generic segment
  (`/api/Logs/Learnlog`) uses the next one (**Learnlog**).
- **Not logs:** words that only contain "log" are ignored: `login`, `logon`, `logout`, `logoff`, `catalog`, `dialog`,
  `blog`, `analog`, `backlog`... (`config.js` → `enterpriseLogs.exclude`).
- **Standard sets:** `config.js` → `enterpriseLogs.types` lists the 12 standard sets with their routes and tab labels:

  | Set | Route |
  |---|---|
  | API Log | GET /api/ApiLog |
  | Luna Log | GET /api/LunaLog |
  | Sys Log | GET /api/SysLog |
  | User Location | GET /api/userlocation/ |
  | User Profile Log | GET /api/UserProfileLog |
  | Learn Log | GET /api/LearnLog |
  | Session Log | GET /api/Sessionlog |
  | Superuser Log | GET /api/Superuserlog |
  | Trouble Tickets | GET /api/Userhelp |
  | User Log | GET /api/Userlog |
  | User Notices | GET /api/Usernotices |
  | User Session | GET /api/Usersession |

  A registered endpoint whose path names a standard set belongs to it even without "log" in it (User Location,
  Trouble Tickets, User Notices, User Session). A standard set an app hasn't registered is still shown, dimmed, and
  tried at `<host>` + its route.
- **Reading a set:** its GET endpoint that lists records (path ends at the set name, no `{parameters}`). A set with only
  POST or `GET /{id}` endpoints shows "–": nothing to list. The page shows which URL it used and every endpoint in the set.
- Newest first by the record's own date field (e.g. `loggedDate`, `createdDate`), else by id. The answer can be a plain
  array or wrapped (`items`, `data`, `value`...). If an app has several hosts, their records are merged.
- If a log answers 401/403, paste a bearer token in the box and Reload; it's only kept while the page is open.
- **Check all logs** reads every category and shows its count on the section header (`!` = couldn't be read; open it for the reason, `–` = nothing to list).
- The applications' APIs must allow the auditor's origin in CORS, like the audit API.
- The whole log is downloaded and sorted in the browser. For very large logs, add paging/`top` support on the API side.

## Welcome page (public)
The summary counts include an **Infrastructure** row: web servers, web farms, database servers and databases, each with
its Infra Audit fail/warn totals, plus total infra findings. A route that isn't deployed shows "–".

`index.html` opens without signing in (its tag is `<script src="auth.js" data-public>`); every other page still requires
a signed-in auditor. The summary counts show for everyone. A top bar has:
- **Help**: trouble tickets (`/api/Userhelp`), newest 25.
- **Notices**: `/api/usernotices`, newest 25.
- **Profile**: the signed-in user's record from `/api/userprofile`, matched on user id, email or user name.
- **Sign in**, or when signed in: the user name, **Open app** (Dashboard) and **Sign out** (stays on Welcome).

Help, Notices and Profile need a signed-in user. Signed out, clicking one does nothing but show a login error
("Please sign in to view Notices") and then take you to the sign-in form; after signing in, the one you clicked opens
automatically. Every link from Welcome to an inside page (Run an audit, Add an API, the counts, Open app) also checks
the login in JavaScript first: signed out, it goes to Sign in and then on to the page that was clicked. Each inside page
also checks for itself. Click a row in Help or Notices for every field. Routes are in `config.js` (`userhelp`, `usernotices`, `userprofile`).

## Sign in
Sign-in goes through the API (build 5.2). `login.html` POSTs `{ "username", "plainPassword" }` to each path in
`config.js` → `auth.loginPaths` (`/api/Auth/signin`, then `/api/Auth/login`), moving to the next one only on 404/405.
The answer's `token`, `userId`, `userRole`, `userUsername`, `userEmail`, name and `sessionId` are kept in localStorage:

| Key | Signed in | Signed out |
|---|---|---|
| `userid` | the user's id | `901` |
| `token` | the JWT from the API | removed |
| `role`, `username`, `email`, `fullname`, `sessionid` | from the API | removed |
| `testlogin` | `1` for the test login | removed |

- A page opens only if `userid` is set (and isn't `901`) and there's a token that hasn't passed its `exp`. Pages check again every
  minute, on Back/Forward and when another tab signs out; an expired session goes to sign-in with "Your session expired".
- **Roles:** `auth.allowedRoles` lists the roles allowed in (any case). Empty (the default) lets in any account that signs in.
- A wrong username or password shows one message for both, so the page doesn't reveal which usernames exist.
- **Sign out** clears the session and, for API users, calls `PUT /api/Users/logout/{token}?id=-1` (`auth.logoutPath`) to mark the
  Usersession complete. `id=-1` because that route matches `Token == token || Userid == id` and needs an `id`.
- The sidebar shows the user name (hover for name, email and role), with a **test** badge for the test login.
- The Weather page starts with the signed-in user's token.

### Test login: audit / audit
`auth.testLogin` is a single hard-coded user: **audit / audit** signs in without calling the API, as user `10000` / role `auditor`, and uses
the fixed test token in `auth.testLogin.token` (a real Usersessions token). Its expiry isn't checked, so it keeps working as long as the API
accepts the token. **Anyone who can open `config.js` can read that token and use it against the API**, so set `testLogin.enabled: false` before
the site is public.

This is still a client-side check: it decides which pages are drawn, it doesn't protect the API. The API must check the token on every
route that needs one. The API calls CockyAuditor makes don't send the token yet.

## Running
Open `index.html` in a browser, or host the folder anywhere static (Azure Static Web Apps, App Service, IIS).
The API must allow the site's origin in its CORS policy. For example, in ASP.NET:

```csharp
builder.Services.AddCors(o => o.AddPolicy("auditor", p => p
    .WithOrigins("https://your-auditor-site", "http://localhost:5500")
    .AllowAnyHeader().AllowAnyMethod()));
app.UseCors("auditor");
```

## Apps (Applications)
An application (`Enterprise.Models.Application`, `/api/Application`) uses one or more host APIs, and a host can be used by
more than one application. The link is `ApplicationApi` (`/api/ApplicationApi`: `applicationId` + `apiHostId`), so host
and endpoint records are unchanged. Routes are set in `config.js` (`apps`, `apphosts`).
- **Apps** page: add/edit/delete applications; **Link hosts** picks the hosts an application uses (shows each host's endpoint
  count and which other applications use it). Adding an application opens Link hosts straight away. Deleting one removes its
  ApplicationApi links first.
- An application's **endpoints** = every endpoint whose `apiHostId` is one of its hosts. A host linked twice counts once.
- The Apps grid's **Endpoints** column shows the total (links to them), active and never-audited counts, and a **Total** row
  under the grid adds them up for the apps shown (an endpoint on a host shared by two apps counts once), with a note of
  endpoints on hosts not linked to any app.
- **Dashboard**: an Apps tile and an Apps table (hosts, total endpoints, active endpoints, last audited), largest first,
  plus a note listing hosts that aren't linked to any application. Click one to see its endpoints.
- **Endpoints**: the filter lists applications as well as hosts (`apiendpoints.html?app=<id>`).
- If the Application routes don't answer (404), these pages show a note and everything else works as before.

## Auditing a host (one click)
Hosts → **Audit host** (or Run Audit → pick a host → **Walk Swagger & audit host**):
1. Reads the host's `swagger.json` from `apiHostUrl` using the saved Swagger login. If there isn't one, or it's rejected,
   it asks for it (and can save it on the host).
2. Registers any operations that aren't endpoints yet. Each new endpoint gets, in order of priority:
   Swagger's per-operation values (method, path, tag, auth) → the **host's default fields** → a filled-in endpoint on the
   host for anything the host leaves blank. The host's `family` is used when an operation has no tag.
3. Ticks every active endpoint on the host that isn't covered by an exception, and runs the audit.
4. Save the run to History as usual.

## How existing endpoints are recognised (no duplicates)
Before inserting anything, Audit host and Sync Swagger reload every endpoint from the database and compare it with each
Swagger operation. An endpoint counts as the same API if its `apiHostId` is the host **or** its `url`/`apiRoot`/`fqdn`
points at the same server (default ports ignored). It counts as the same operation when:
1. method + path match after normalising: method from `type`, `endpointType` or the first word of `description` (any case);
   path from a full-URL `url`, or `apiRoot` + `url`, with the Swagger base path removed, lower case, `{anyName}` = `{}`,
   no trailing slash, no query string; or
2. its `swaggerOperationId` matches; or
3. it has no method stored and the path matches (one operation per record).

Only operations with no match are inserted, and an operation listed twice in Swagger is inserted once. Matches that aren't
linked to the host (`apiHostId` blank or 0) get `apiHostId` set instead of a copy being made. Records that are already
duplicates of each other are reported, not deleted.

**Confirmation:** Audit host shows how many records will be inserted and linked (with the list) and waits:
*Insert N records & audit*, *Audit without adding* (writes nothing), or *Cancel*. It doesn't ask when there's nothing new.
The Sync Swagger wizard shows the same counts on its review step, and re-checks the database just before importing.

## Host defaults
The host record carries the same descriptive fields as an endpoint (department/`groupDescription`, building, ops
contacts, auditor, DBA, platform, network, Azure...). They're the template for every endpoint registered from that host.
- In host defaults, 0 in an id field (`auditorId`, `groupId`, `buildingId`, `dbaId`, `businessUnitOwnerId`) and `false`
  in a yes/no field mean "not set", so they don't overwrite anything.
- On the Hosts page, **Edit** shows the core fields (API, Swagger login, status); **Details** opens the descriptive
  fields (ownership, contacts, auditor, platform, Azure, network) in their own dialog.
- **Audit host** copies the host's filled-in details onto new endpoints *and* updates existing endpoints of that host
  (only fields the host has filled in and that differ).
- **Push details** does the same copy on demand, without auditing.
- The import wizard can save the details you enter onto the host (blank host fields only).
- A host counts as inactive if either `active` or `isActive` is false.

## Required fields on create
If the API rejects a new record with 400 because a non-nullable field was left blank (e.g. a C# `string` or `Guid`
property), CockyAuditor fills just those fields ("N/A", 0, false, now, or an all-zero Guid) and retries, then tells you
which ones it filled. Making those properties nullable (`string?`, `Guid?`) in the API avoids this.

## Adding an API from Swagger
Hosts (or Endpoints) → **+ Add API from Swagger**:
1. Enter the API URL and its Swagger username/password. CockyAuditor downloads `swagger.json` with HTTP Basic auth
   (it tries `/swagger/v1/swagger.json`, then other common locations). OpenAPI 3 and Swagger 2 are supported.
2. Review every operation (method, path, tag, whether Swagger declares authentication), then import.
   A host is created (with the Swagger login, if you tick the box) and one endpoint per operation:
   `type` = HTTP method, `url` = path, `family` = tag, `swaggerOperationId`, `hasAuthEnabled`/`authType` from the security schemes.
   `apiRoot` gets the full root URL (e.g. `https://myapi.azurewebsites.net/`), matching endpoints entered by hand.
   **Endpoint details**: Swagger has no contacts, DBA, building, business unit or Azure info, so the wizard copies those from an
   endpoint you've already filled in (by default the most recently edited one on the same API), and lets you edit them first.
3. **Sync Swagger** on a host re-reads the file: new operations can be imported, and ones no longer in Swagger are marked inactive.

If a required column is blank, the import retries once with "N/A"/0 in the fields the API reported, and tells you which.
If the browser can't fetch the file (CORS), paste or upload swagger.json in the wizard instead.

For the browser to fetch a protected swagger.json from another site, the API must:
- allow the auditor's origin in CORS, **including the `Authorization` header**, and
- run `app.UseCors(...)` **before** the Swagger Basic-auth middleware, so the browser's OPTIONS pre-check isn't rejected with 401.

## How Run Audit checks an endpoint
It **only ever sends GET requests**, and never calls POST/PUT/PATCH/DELETE operations. For those, and for paths with
`{parameters}`, a 400/404/405 answer counts as "route reachable". A GET that returns 2xx without credentials is
a **Fail** when Swagger says the endpoint needs authentication, and a **Warning** ("open endpoint") when it doesn't.
Up to 6 endpoints are checked at once. Filter by host with the dropdown, or open `apiaudit.html?host=<id>`.

## How Run Audit builds a target URL
1. If the endpoint's `url` is a full `http(s)://` address, it uses that.
2. If `apiRoot` is a full `http(s)://` address, it uses `apiRoot` + `url`.
3. Otherwise, if `apiHostId` points to a Host, it uses `apiHostUrl` + `apiRoot` (as a base path) + `url`.
4. Otherwise it uses `https://` + `fqdn` / `hostName` / `iPv4Address` (+ `primaryPort`) + path.

You can override the URL on the Run Audit page before running.

## What a saved audit contains
Each run saves **one** ApiAuditResults record: host and endpoint totals (secure = tested over HTTPS),
auditor name and ID, and notes (your note + one line per endpoint with its verdict, status, time and any warnings or failures).
If "stamp" is ticked, each audited endpoint gets `lastAuditDate`, `auditResultId` and `endpointSecure`,
and each audited host gets `lastAuditDate` and `lastAuditId`.

## Dates
The API returns dates without a time zone (e.g. `2026-09-22T21:10:35.369`). CockyAuditor treats these as UTC and shows them in your local time.

## Exceptions
An active, unexpired exception removes endpoints from Run Audit (they're left unticked and badged).
Each exception can be scoped by any mix of these fields, and every field that is set must match:

| apiAuditId | apiRootUrl | apiEndpointPath | Excludes |
|---|---|---|---|
| 5 | | | endpoint #5 only |
| | `https://api3.capitoltechnology.net` | | every endpoint under that API |
| | `https://api3.capitoltechnology.net` | `/api/Student/*` | everything under that path on that API |
| | | `/api/Student/*` | that path on any API |

Blank, null, or Swagger's placeholder `"string"` all count as "not set".
`*` is a wildcard; a path without one must match exactly (a trailing slash is ignored). When `apiRootUrl`
includes a base path (e.g. `https://host/v1`), `apiEndpointPath` is relative to it. Matching is case-insensitive.

Expired exceptions are flagged in red on the Exceptions page and Run Audit.

### Host exceptions
A host exception (`ApiHostException` table, route set in `config.js` as `hostexceptions`, `/api/ApiHostException`)
takes a whole host out of auditing while it's `active` and not past `expirationDate`:
- **Audit host** / **Walk Swagger & audit host** are blocked for that host (Swagger isn't even fetched), with the reason shown.
- In any run, including an all-hosts run, the host's endpoints are unticked and locked, and a banner lists the excepted hosts.
- Hosts shows an **Excepted** badge; **Except host** opens a new host exception prefilled for that host.
- Exceptions has a **Host exceptions** tab; the Dashboard adds "+N hosts" to the active exception count.

Ids are generated by Azure SQL and are never sent when creating a record.

## Infrastructure (build 4.9)
Four inventories, each with its own page under **Infrastructure** in the sidebar:

```
Application ─┬─ WebServerId ──> WebServers
             ├─ WebFarmId ────> WebFarms ──< WebFarmServers >── WebServers
             ├─ (DatabaseId) ─> Databases ──> DatabaseServers
             └─ APIs (ApplicationApi) ──> Apihosts
```

- **Web Servers**: add/edit/delete. The **Farms** column shows which farms a server is in. Deleting a server removes its
  farm memberships first (put back if the delete fails). A server an application points to can't be deleted.
- **Web Farms**: **Members** picks the farm's servers. WebFarmServers has no PUT, so saving POSTs new links and DELETEs
  removed ones. If a server is linked to the same farm twice, saving removes the extra links. Adding a farm opens
  Members straight away. Deleting a farm removes its member links first.
- **Database Servers**: **Databases** opens the server's databases. A server with databases can't be deleted.
- **Databases**: filter by server (`databases.html?server=<id>`); **+ Add** on a filtered list puts the new database on
  that server. New databases start with Backup required ticked (the SQL default). **Used by** lists applications once
  `Application` has a `DatabaseId` (it doesn't yet, so the column shows n/a).
- **Apps**: the **Runs on** column shows each application's web farm, web server (and database, when available), and the
  edit form has an **Infrastructure** section with the dropdowns.
- **Dashboard**: an Infrastructure card with the count and fail/warn totals of each kind.
- A route that answers 404 shows a setup note instead of an error. Any route in `config.js` can be a full `https://` URL
  if these tables live on a different API from `apiBaseUrl`.

### Infra Audit
Checks the inventory records against the policy in `config.js` → `infraAudit`. It reads the inventory records only and
never connects to the servers. Inactive servers and databases, and disabled farms, show as "not audited".
Each inventory page's **Findings** column shows the same result (hover for the list, click to open it in Infra Audit).

| Applies to | Fail | Warn |
|---|---|---|
| Everything | TLS below `minTlsVersion` (the lowest version listed counts: `1.0/1.1/1.2` fails) or any SSL; plain `http://` URL on a public-facing item; version matched by an `endOfSupport` rule at level fail | blank `requiredFields` (owner, support team, environment, inventory id); TLS not recorded; plain `http://` URL on an internal item |
| Web server | | OS or type/version not recorded; no hostname/IP; environment differs from its farm |
| Web farm | no members; a member allows old TLS | backend pool count ≠ members; inactive, missing or duplicate members; members in another environment; no health probe; no load balancer |
| Database server | public facing; no backup solution while a database on it requires backup | no backup solution; SIMPLE recovery with backups required; platform/version not recorded |
| Database | PII/PHI/PCI without encryption, without a classification, or with a public classification (`publicClassifications`); on a public-facing server; backup required but the server has none; server missing | server inactive; no classification |
| Application | | points to a missing or inactive server, farm or database; web server not in the app's farm; web server, farm or database server in a different environment |

`endOfSupport` rules are regular expressions matched against the OS (web servers), "type version" (web servers) and
"platform version" (database servers). The defaults cover Windows Server ≤ 2012 R2, Windows 10 and older, CentOS,
RHEL ≤ 6, Ubuntu ≤ 18.04, IIS ≤ 8.5, Apache 2.2, Tomcat ≤ 8.5, SQL Server ≤ 2016 (by name or 13.x build number),
MySQL ≤ 8.0 and PostgreSQL ≤ 13 as fail, plus Windows Server 2016, RHEL 7, Ubuntu 20.04 and SQL Server 2017 as warn
(support dates as of September 2026).

**Re-run**, **Copy report** (text summary) and **Download CSV** (one row per finding) are on the page. Results aren't
saved to the database: ApiAuditResults only has endpoint/host totals. To keep a history, add an InfraAuditResults table.

### Field names
The forms use the names the API returns. When a table is still empty they fall back to `schema.js`, which assumes
ASP.NET's camelCase (`TLSVersion` → `tlsVersion`, `AzureURL` → `azureURL`, `CName1` → `cName1`, `ContainsPII` → `containsPII`).
The checks match field names in any case.

### Schema notes
- `WebFarmServers` has no unique key on (WebFarmId, WebServerId); the UI avoids duplicates and Infra Audit flags them.
  Consider `CONSTRAINT UQ_WebFarmServers UNIQUE (WebFarmId, WebServerId)`.
- `WebFarms` uses `Enabled`; the other tables use `Active`.
- `Enterprise.Models.Application` has `WebServerId` and `WebFarmId` but no `DatabaseId`. Add `public int? DatabaseId { get; set; }`
  (and the column) and the Apps form, Runs on column and Databases → Used by pick it up automatically.
