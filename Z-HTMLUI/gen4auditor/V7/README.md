# CockyAuditor

A static web front end for the audit API at
`https://cockyanalyticsg4-cqfrgacteud3c2h3.westus3-01.azurewebsites.net`.

## Version
Current: **Build 4.7 · Public welcome**. The version and build history live in `config.js` (`version`, `builds`);
add each new build to the top of `builds` and the home page and sidebar pick it up.

| Build | Title |
|---|---|
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
| Sign in | login.html | none (hard-coded test user) |
| Welcome (public) | index.html | quick counts from all of them; top bar: /api/Userhelp, /api/usernotices, /api/userprofile |
| Dashboard | apidashboard.html | all of them |
| Apps | apiapps.html | /api/Application, /api/ApplicationApi |
| Hosts | apihosts.html | /api/Apihosts |
| Endpoints | apiendpoints.html | /api/Apiaudit |
| Run Audit | apiaudit.html | reads Apiaudit + Apihosts, writes ApiAuditResults |
| Exceptions | apiexceptions.html | /api/AuditExceptions (+ /active, /audit/{id}) and /api/ApiHostException |
| History | apihistory.html | /api/ApiAuditResults |
| Enterprise(9) | enterprise9.html | each application's own API: GET /api/<Log> for every Enterprise(9) log |

## Files
- `auth.js`: sign-in guard, loaded first in every page's `<head>`.
- `config.js`: API base URL, routes, timeouts, copyright line. (Replaces config.json, which broke when pages were opened from disk.)
- `schema.js`: field lists and types for each resource, plus `summarizeAudit` / `buildAuditResultPayload`, the summary record Run Audit saves.
  All four resources match the real API models.
- `app.js`: shared layout, API client, tables and add/edit dialog.
- `swagger-import.js`: the "Add API from Swagger" / "Sync Swagger" wizard.
- `site.css`: styles.

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
Every page loads `auth.js` first in `<head>`. Before anything is drawn it checks localStorage:

| Key | Signed in | Signed out |
|---|---|---|
| `userid` | `10000` | `901` |
| `role` | `auditor` | removed |
| `username` | `auditor` | removed |
| `email` | `auditor@capitoltechnology.net` | removed |

A page opens only if `role` is `auditor` and `userid` is set and isn't `901`. Otherwise it goes to
`login.html?next=<page>`, and after signing in you land back on that page. The check runs again on Back/Forward and
when another tab signs out. **Sign out** (sidebar and Home) sets `userid` to `901` and removes the other keys.

Test login: **audit / audit**. It's checked in the browser only and never calls the API, so anyone who reads the
source or edits localStorage can get in. It keeps casual visitors out; replace it with a real API login before this is public.

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
