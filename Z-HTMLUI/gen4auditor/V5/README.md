# CockyAuditor

A static web front end for the audit API at
`https://cockyanalyticsg4-cqfrgacteud3c2h3.westus3-01.azurewebsites.net`.

## Version
Current: **Build 4.1 · Apps**. The version and build history live in `config.js` (`version`, `builds`);
add each new build to the top of `builds` and the home page and sidebar pick it up.

| Build | Title |
|---|---|
| 4.1 | Apps |
| 4.0 | Host Exceptions |
| 3 | AutoWalk of Swagger / Bug Fixes |
| 2 | Home Page |
| 1 | Base |

## Pages
| Page | File | API |
|---|---|---|
| Welcome | index.html | quick counts from all of them |
| Dashboard | apidashboard.html | all of them |
| Apps | apiapps.html | /api/Application, /api/ApplicationApi |
| Hosts | apihosts.html | /api/Apihosts |
| Endpoints | apiendpoints.html | /api/Apiaudit |
| Run Audit | apiaudit.html | reads Apiaudit + Apihosts, writes ApiAuditResults |
| Exceptions | apiexceptions.html | /api/AuditExceptions (+ /active, /audit/{id}) and /api/ApiHostException |
| History | apihistory.html | /api/ApiAuditResults |

## Files
- `config.js`: API base URL, routes, timeouts. (Replaces config.json, which broke when pages were opened from disk.)
- `schema.js`: field lists and types for each resource, plus `summarizeAudit` / `buildAuditResultPayload`, the summary record Run Audit saves.
  All four resources match the real API models.
- `app.js`: shared layout, API client, tables and add/edit dialog.
- `swagger-import.js`: the "Add API from Swagger" / "Sync Swagger" wizard.
- `site.css`: styles.

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
