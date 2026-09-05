# BankStatement.Demo — Enhancement Backlog

> Assessment and enhancement stories for the `BankStatement.Demo` API.
> Status of code at time of writing: branch `feature/seeded-data-wip` (work in progress).

## Maturity assessment (~2–4 YOE, early-to-mid level)

**Strengths already in place**
- Modern .NET 10 idioms — primary constructors, nullable + implicit usings.
- JWT auth with **refresh-token rotation and revocation** (`Services/AuthService.cs`).
- **BCrypt** password hashing (not plaintext/SHA).
- Thoughtful EF Core mapping — `HasPrecision(18,2)` on money, unique index on `StatementId`, `jsonb` columns, owned-entity `ToJson()`, `EnableRetryOnFailure` resilience.
- Clean QuestPDF document composition.

**Gaps pulling the rating down**
| Signal | Location | Status |
|---|---|---|
| ~~Hardcoded absolute path for seed data~~ | ~~`Data/Seeded/SeededData.cs:18`~~ | ✅ Resolved — `SeededData` and its JSON files were removed (dead code once `GetStatements` was disabled) |
| Secrets committed to source (JWT key, DB password) | `appsettings.json:5`, `appsettings.json:17` | Open — see A1 |
| ~~EF persistence layer exists but is never used by the API (serves static JSON instead)~~ | ~~`Controllers/BankStatementController.cs`, `Data/AppDbContext.cs`~~ | ✅ Resolved — endpoints now go through `IUnitOfWork`/`BankRepository` against Postgres |
| ~~No service/repository abstraction — controller calls `static SeededData` directly~~ | ~~`Controllers/BankStatementController.cs:16`~~ | ✅ Resolved — `IUnitOfWork` + `IBankRepository` abstraction now in place |
| ~~`async` method with no `await`~~ | ~~`Controllers/BankStatementController.cs:14`~~ | ✅ Resolved — the offending `GetStatements` action was dead code and has been removed |
| PDF endpoint has no `[Authorize]` and no input validation | `Controllers/BankStatementController.cs:21` | 🟡 Partially resolved — `[Authorize]` now applied at controller level; input validation still open |
| ~~`WIP` domain models returned directly to clients; two competing shapes~~ | ~~`Models/BankStatement/WIP/*`~~ | ✅ Resolved — see B4 |
| ~~Dead template scaffolding~~ | ~~`Controllers/WeatherForecastController.cs`, `WeatherForecast.cs`~~ | ✅ Resolved — see B6 |
| No tests, structured logging, validation, CORS, rate limiting, or health checks | — | Open — see EPIC F, A4, D5 |

---

## EPIC A — Security & best practices

### A1 — Move secrets out of source control
Use `dotnet user-secrets` in dev and environment variables / Key Vault in prod. Fail fast at startup if the JWT signing key is missing or shorter than 32 bytes.
- **AC:** No secret literals remain in the repo; app refuses to boot with a weak/missing key.

### A2 — Lock down data endpoints
Add `[Authorize]` to statement and PDF endpoints; scope every read to the authenticated user.
- **AC:** Anonymous calls return 401; user A cannot read user B's statements.

### A3 — Global error handling
Add `UseExceptionHandler` + `ProblemDetails`; never leak stack traces to clients.
- **AC:** Unhandled errors return RFC-7807 JSON with a correlation id.

### A4 — Rate-limit authentication
Add `AddRateLimiter` on `/login` and `/refresh` to blunt brute-force attempts.
- **AC:** N failed logins per minute from a client returns 429.

---

## EPIC B — Architecture & design patterns

### B1 — Repository + service layer for statements
Replace `static SeededData` with `IBankStatementService` / `IBankStatementRepository`, registered in DI.
- **AC:** Controller has zero static/file calls; the layer is unit-testable via mocks.

### B2 — Remove the hardcoded seed path
Load seed data via `IWebHostEnvironment.ContentRootPath`, embedded resources, or EF seeding — never an absolute path.
- **AC:** Runs on a fresh clone / CI with no path edits.

### B3 — DTO boundary + mapping 🟡 Partially done
Introduce request/response DTOs with Mapperly/AutoMapper; stop returning `WIP` domain models.
- **AC:** The public API contract is decoupled from persistence entities.
- **Status:** `BankStatementDto` mapping now goes through an AutoMapper `Profile` (`Mapping/BankStatementProfile.cs`) instead of hand-rolled object initializers in the controller. `WIP` models are no longer returned/referenced anywhere (see B4).

### B4 — Consolidate the model shapes ✅ Done (enum still open)
Merge `BankStatement1` / `BankStatement2` into one canonical model; delete the `WIP` namespace; replace magic strings like `"CREDIT"` with an enum.
- **AC:** One statement model; no `WIP` namespace; transaction type is a strongly-typed enum.
- **Status:** `BankStatement1`/`BankStatement2` and the entire `Models/BankStatement/WIP` namespace have been deleted, along with the orphaned `BankStatement.cs`/`Account.cs`/`Transaction.cs` models and the `SeededData` loader that referenced them. `BankStatementDto`/`BankStatementEntity` are now the single canonical shapes. Remaining: `TransactionEntity.Type` is still a `string` (`"CREDIT"`/`"DEBIT"`) — converting to a strongly-typed enum is still open.

### B5 — Options pattern for JWT settings
Bind `JwtSettings` via `IOptions<JwtSettings>` instead of parsing raw configuration strings in the service.
- **AC:** No `double.Parse(config[...])` calls; settings validated on startup.

### B6 — Remove dead scaffolding ✅ Done
Delete `WeatherForecastController` and `WeatherForecast`.
- **Status:** `WeatherForecastController`/`WeatherForecast` were already removed; the stale `GET .../weatherforecast/` request left in `BankStatement.Demo.http` has now been removed too.

---

## EPIC C — Persistence (connect the EF layer)

### C1 — Wire the database into the request flow 🟡 Partially done
Ingest → persist `BankStatementEntity` → serve statements from Postgres, so the entities and migration are actually exercised.
- **AC:** `GET /statements` returns rows from the database, not static JSON.
- **Status:** `POST create` persists via `IUnitOfWork`/`BankRepository`, and PDF endpoints (`GET {id}/pdf`, link-based redemption) read the entity back from Postgres (including `Transactions`). Still open: there's no plain `GET /statements` listing endpoint — only single-record reads by id exist so far.

### C2 — Idempotent database seeding
Seed via a seeding service or EF `HasData` instead of runtime JSON file reads.
- **AC:** Re-running seeding does not create duplicates.

### C3 — Per-user ownership
Add a `UserId` foreign key to statements and enforce it in every query.
- **AC:** Queries are always filtered by the current user's id.

---

## EPIC D — Performance & scalability

### D1 — Pagination, filtering, sorting on statement listings
- **AC:** `?page=&pageSize=` with a total count; the server caps page size.

### D2 — Async all the way
Fix async-without-await; use async file/DB I/O; add `AsNoTracking()` on read queries.
- **AC:** No sync-over-async; read queries are non-tracking.

### D3 — Offload heavy PDF generation
Move large PDF generation to a background queue (Hangfire / `System.Threading.Channels`) and cache results; return a job id to poll, then download.
- **AC:** The request thread is not blocked on large PDF renders.

### D4 — Response caching / ETag on statement reads
- **AC:** Unchanged statements return 304 on conditional GET.

### D5 — Health checks and container readiness
Add `AddHealthChecks().AddNpgSql()`, a Dockerfile, and docker-compose for Postgres.
- **AC:** `/health` reports DB connectivity; the app runs via `docker compose up`.

---

## EPIC E — PDF storage & download

### E1 — Persist generated PDFs to a local folder and expose a download link ⭐
**Decision:** Storage target = **local disk folder** (configurable path), built behind an `IPdfStorage` abstraction so cloud blob storage can be swapped in later without touching the controller.

**Story:** As an authenticated user, when I generate a bank-statement PDF, the file is saved to a configured storage folder and I receive a stable download URL, so I can re-download it later instead of only receiving a one-time stream.

**Acceptance criteria**
- A `Storage:PdfPath` setting (in `appsettings`, overridable by env var) defines the output folder; the folder is created if missing.
- `POST .../createpdf` generates the PDF, saves it as `{statementId}/{generatedFileId}.pdf`, and returns a document id + download URL (not the raw bytes).
- `GET .../pdf/{id}` streams the stored PDF with `application/pdf` and a sensible filename; returns 404 if not found.
- Both endpoints require `[Authorize]`; a user can only download their own PDFs.
- Filenames/ids are non-guessable (GUID) and path-traversal safe.

**Technical notes / tasks**
- Define `IPdfStorage` with `SaveAsync(stream/bytes) -> id` and `OpenReadAsync(id) -> stream`; implement `LocalDiskPdfStorage` bound to `Storage:PdfPath`.
- Register `IPdfStorage` in DI; inject into the controller/service (removes the current static, stream-only flow).

### E2 — Expiring, single-use PDF download links ✅ Done
**Story:** As an authenticated user, I can request a shareable PDF link that expires after a fixed window and can only be redeemed once, so recipients get time-boxed, non-reusable access without needing to authenticate themselves.

**Acceptance criteria**
- `GET {id:guid}/GetPdfLinkToken` (authenticated) returns a signed URL valid for 15 minutes.
- `GET pdf?fileName=&expiresAt=&signature=` (anonymous) validates the HMAC signature + expiry, then streams the PDF.
- The link can only be redeemed once — a second request with the same signature returns `401 Unauthorized`.

**Implementation notes**
- `Services/PdfLinkService.cs` — signs/validates `fileName:expiresAt` payloads via `HMACSHA256` (`FileSigningKey` from configuration), using constant-time comparison (`CryptographicOperations.FixedTimeEquals`) to avoid timing attacks.
- `Entities/PdfLinkToken.cs` + `AppDbContext.PdfLinkTokens` — tracks issued signatures with `ExpiresAt`/`UsedAt` so redemption can be marked consumed (`PdfLinkService.TryConsumeLinkAsync`), enforcing single-use since a bare HMAC/JWT alone can't be revoked without server-side state.
- Known gap: expiry (15 minutes) is currently a hardcoded constant rather than configurable; no cleanup job exists yet for expired/used `PdfLinkToken` rows (table grows unbounded).

- Add validation on the incoming statement body before rendering.

---

## EPIC F — Quality & observability

### F1 — Test project
Unit tests for `AuthService` and PDF generation; integration tests via `WebApplicationFactory` + Testcontainers-Postgres.

### F2 — Structured logging
Add Serilog with correlation ids across requests.

### F3 — Request validation
Add FluentValidation for all request DTOs.

### F4 — Honor nullable reference types
Use `required` members / initializers so `Nullable=enable` is actually enforced (no non-null string properties left uninitialized).

### F5 — CI pipeline
Build + test + `dotnet format` check on pull requests.
