# CockyAuditor

A static web front end for the audit API at
`https://cockyanalyticsg4-cqfrgacteud3c2h3.westus3-01.azurewebsites.net`.

## Version
Current: **Build 5.2 · API sign-in & Weather test**. The version and build history live in `config.js` (`version`, `builds`);
add each new build to the top of `builds` and the home page and sidebar pick it up.

| Build | Title |
|---|---|
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
| Welcome (public) | index.html | quick counts from all of them; top bar: /api/Userhelp, /api/usernotices, /api/userprofile |
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
| Weather (Utilities) | weather.html | /api/weather (+ /api/Usersession for Recent sessions) |
| Enterprise(9) | enterprise9.html | each application's own API: GET /api/<Log> for every Enterprise(9) log |

## Files
- `auth.js`: sign-in guard, loaded first in every page's `<head>`.
- `config.js`: API base URL, routes, timeouts, copyright line. (Replaces config.json, which broke when pages were opened from disk.)
- `schema.js`: field lists and types for each resource, plus `summarizeAudit` / `buildAuditResultPayload`, the summary record Run Audit saves.
  All four resources match the real API models.
- `app.js`: shared layout, API client, tables and add/edit dialog.
- `infra.js`: Infrastructure checks (Infra Audit rules), loading, and the Findings badges.
- `swagger-import.js`: the "Add API from Swagger" / "Sync Swagger" wizard.
- `interfaces.js`: back-end interface discovery (SystemConsole), matching, and endpoint suggestions.
- `db/ApiInterfacesAudit-5.0.sql`: the database change for build 5.0.
- `site.css`: styles.

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
