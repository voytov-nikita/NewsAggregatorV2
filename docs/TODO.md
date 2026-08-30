# TODO

## NewsService

### ✅ News categories — DONE (single-category, enum-backed)

Implemented as a `short`-backed `NewsCategory` enum on `NewsEntity` (not a separate
taxonomy table — chosen for simplicity given small fixed set; see chat 2026-05-16).
`Tags string[]` (jsonb) also stored as raw values from the source. Resolver in
`NewsService.BLL.Helpers.NewsCategoryResolver` maps incoming tags + publisher
fallback into the canonical enum. Migration `AddCategoryAndEngagementFields`
backfills existing rows.

**Follow-ups (out of scope for the first cut):**
- Surface multi-category articles. Current resolver picks one; if a news item
  legitimately belongs to two (e.g. "Frontend" + "State"), only the first wins.
  When this becomes a real need, add a separate `NewsTags` table or M:N to a
  `Category` table — that's the original plan in this slot.
- Admin UI for managing the synonym map without redeploys.

### ✅ News rating — DONE

`POST /api/v1/news/{id}/vote` (`news.vote` permission, body `{ value: 1 | -1 | 0 }`,
0 retracts) writes to the `Votes` table, which carries `UNIQUE (NewsId, UserId)` —
that index, not application code, is what makes voting idempotent. `Likes` /
`Dislikes` on `NewsEntity` stay denormalized and are recounted from `Votes` inside
the same transaction. The feed wires the vote buttons to it; anonymous visitors are
sent to the login page instead.

The `X-Device-Id` interim for anonymous voting was dropped — auth landed first, so
it would only have created state to migrate later.

**Still open:**
- `myVote` in the list response, so the feed can render the caller's own vote on
  load rather than only after they click. Deferred with the Phase B list rework
  below, which changes the response shape anyway.
- Rate limiting. The unique index caps a user at one vote per article, but nothing
  caps how fast an account can vote across many articles.

### News list — totalCount + extended filtering (Phase B)

The response is currently a bare array; the client can't render accurate pager
state. Plan:
- Change `GET /api/v1/news` response to `{ items: NewsResponse[], totalCount: int }`.
- Add multi-value filters from the prototype filter rail:
  `categories[]`, `sources[]`, `dateFrom`, `dateTo`, `readTimeMin/Max`,
  `likesMin/Max`, `hasComments`.
- Extend `NewsOrderField` with `MostLiked`, `MostDiscussed`, `MostViewed`
  (the last one requires view tracking — separate task).

### User follows / subscriptions

Backend for the existing client-side Subscriptions screen (currently localStorage:
`na-follows`). **Unblocked** — AuthService ships a stable `Guid` user id, and the
screen is already behind `authGuard`. The anonymous `X-Device-Id` table was never
built, so there is no "claim follows on login" migration to write.

**Plan:**
- New `UserFollow` table (PostgreSQL): `(UserId, Kind, Key)` unique.
  Kind ∈ {Category, Source, Author}.
- `GET /api/v1/me/follows` → `{ categories: string[], sources: string[], authors: string[] }`
- `POST/DELETE /api/v1/me/follows/{kind}/{key}` → 204
- Frontend `FollowsService` swaps localStorage for HTTP, keeps the same signal API.
- `Authors` are TBD: requires an `Author` column on `NewsEntity` first.

### Saved articles

Same shape as Follows — client-side only today (`na-saved` localStorage).
**Unblocked** by AuthService; the screen is already behind `authGuard`. Endpoints:
- `GET /api/v1/me/saved` → `NewsResponse[]`
- `POST /api/v1/me/saved/{newsId}` / `DELETE` → 204

### News validation

Server-side validation rules for incoming news from `news-queue` (title/description length,
required fields, URL format, publish date sanity). Currently we trust whatever CrawlerService
sends.

### Normalize publisher storage

Currently the `News` table duplicates publisher info (`PublisherName`, `PublisherLink`, `PublisherGuid`)
on every row. With multiple sources coming from CrawlerService, this duplication grows linearly with
the number of articles.

**Idea:** Extract a separate `Publishers` (or `Sources`) table in NewsService and reference it by FK
from `News`. Each article would store only a `SourceId` / `PublisherId`, and publisher metadata lives
in one row per source.

**Considerations:**
- NewsService currently has no concept of a source — publisher arrives as denormalized fields in
  the `news-queue` message. Either propagate `SourceId` end-to-end (CrawlerService → queue → NewsService),
  or have NewsService upsert a `Publisher` row on first sight of a new `PublisherName`.
- Requires an EF Core migration (`dotnet ef migrations add NormalizePublisher`) and a data backfill
  for existing rows.
- Frontend DTOs may need to join publisher info — keep denormalized at the read model if performance matters.

## CrawlerService

### Cross-source deduplication

Currently, deduplication is based on `GlobalUniqueId` (publisher name + guid from RSS feed).
This only catches exact duplicates from the same source.

When multiple sources are added, the same news story may appear from different publishers
(e.g., Pravda and BBC both report the same event). These are currently treated as separate news items.

**Idea:** Implement cross-source deduplication by comparing:
- News titles (fuzzy matching / similarity score)
- Common tags or categories
- Combination of both

This would allow grouping related articles from different sources
and avoiding duplicate stories in the aggregated feed.

**Considerations:**
- Fan-in pattern may be needed if deduplication requires data from all sources at once
- Alternatively, compare against already-saved news in the database (no fan-in needed)
- Fuzzy matching adds complexity — consider a similarity threshold

### Source management UI (step 2 of source refactor)

After step 1 (universal parser with mapping, sources stored in Mongo) is complete,
add a frontend for managing sources without touching code or the database directly.

**Scope:**
- CRUD API in `CrawlerService.API` for `NewsSource` (list, create, update, delete, enable/disable)
- Server-side validation: URL reachability, cron format, mapping syntax, at least one required selector
- Angular UI for source list and edit form
- Interactive mapping "designer":
  - User pastes a source URL
  - UI fetches and renders a sample response (feed items or HTML page)
  - User clicks on fields in the sample to build the mapping visually
  - Live preview of parsed items against the current mapping
- Hot reload of Hangfire jobs when sources change (no app restart)
- Test endpoint: trigger a one-off crawl and return parsed items for debugging

## NotificationService

### Complete webhooks API

Currently only `POST /api/v1/webhooks` is implemented. The original contract in
`docs/Solution/Solution.md` expects a full CRUD API.

**Scope:**
- `GET /api/v1/webhooks` — list with default pager (OrderBy, OrderDirection, Offset, Take)
- `GET /api/v1/webhooks/{id}` — single webhook
- `PUT /api/v1/webhooks/{id}` — update URL / EventType / enabled flag
- `DELETE /api/v1/webhooks/{id}` — unsubscribe
- Unit tests for BLL and DAL

### SignalR realtime notifications

Add SignalR hub to NotificationService to push realtime events to the frontend
(new news published, webhook delivered/failed, etc.).

**Scope:**
- SignalR hub in NotificationService.API (`/hubs/notifications`)
- Consume `webhook-triggered` queue → broadcast to connected clients
- Angular client: `@microsoft/signalr` integration, in-app toast on new news
- Authentication for hub connections — `Common.Auth` is in place, so the hub can
  validate the same RS256 token; SignalR passes it as an `access_token` query
  parameter for WebSocket transports, which needs an `OnMessageReceived` handler

## ✅ Authorization Service — DONE

AuthService (port 7330, schema `authService` in the shared `local-news` database)
issues RS256 JWTs; every other service validates them through `Common.Auth` against
its JWKS. Authorization is permission-based, not role-based: roles map to
permissions, permissions travel as `permission` claims, endpoints opt in with
`[HasPermission(...)]`.

**Done:**
- ASP.NET Core Identity with `Guid` keys; permissions as real tables
  (`Permissions`, `RolePermissions`) seeded from `Permissions.All` on startup
- RS256 signing with a key that survives restarts, a `kid` derived from the key,
  retired-key rotation, `/.well-known/openid-configuration` + `/.well-known/jwks.json`
- register / login / refresh / logout / me; refresh tokens stored as SHA-256 hashes
  with a `FamilyId` rotation chain and reuse detection
- Refresh token only ever in the `na_rt` httpOnly + Secure + SameSite=None cookie;
  access token 15 minutes, held in memory on the client
- `Common.Auth` wired into all four services; comment authorship, comment
  ownership/moderation checks and news voting are behind permissions
- Angular: `AuthModule`, signal-based `AuthService`, single-flight refresh
  interceptor, `authGuard` / `permissionGuard`, `UserMenu`, permission-aware nav

**Not done, with the trigger for each:**
- **Email confirmation and password reset.** Needs an email transport first;
  NotificationService is the natural home for it.
- **Google / external login.** `AddAuthentication().AddGoogle(...)` plus an
  `AspNetUserLogins` flow. Wanted when sign-up friction actually matters.
- **Access-token revocation.** Logout revokes the refresh token, but the access
  token stays valid for up to 15 minutes — inherent to stateless JWT. A `jti`
  denylist or a `SecurityStamp` version claim checked per request would fix it;
  worth it when instant sign-out becomes a requirement.
- **Refresh-token cleanup job.** Expired and revoked rows are never deleted.
  A Hangfire recurring job, or a `DELETE ... WHERE ExpiresAt < now() - interval`.
- **Double-submit CSRF token.** Rotation plus an explicit CORS origin list is
  enough while the app is localhost-only; needed before a public domain.
- **Per-service audiences.** One audience (`news-aggregator`) serves all four
  services today. Split when a third party integrates.
- **Admin UI for users and roles.** Assigning roles currently means touching the
  database.
- **`user-updated` event.** `CommentEntity.AuthorName` is a snapshot taken at write
  time, so renaming a user does not rename their old comments. RabbitMQ is already
  in place; this is a good exercise.

## Frontend

### News details page (`/article/:id`)

`/article/:id` is referenced by `AuthorizedLayout.activeId` and by the design
prototype, but no route and no component exist — the news list is the only screen.
`CommentsService` is fully written and currently unused by any component.

The backend side is complete: comments carry `AuthorId` / `AuthorName`, writing needs
`comment.write`, editing and deleting are the author's or a moderator's, and the three
comment routes that used to 404 (a leading slash made them absolute) are fixed. So this
is purely a client task:
- `ArticleDetail` component + `/article/:id` route in `NewsModule`, wrapped in
  `<app-page-shell>` with a back action in the `pageHeaderLeading` slot
- Article body, source badge, publisher link, read time, publish date
- Vote buttons bound to `NewsVotesService`, same gating as the feed; `GET .../vote`
  gives the caller's own vote on load, which the list endpoint still cannot
- An actions menu on the page: Save / Unsave (`SavedArticlesService`), Follow source
  (`FollowsService`), Copy link, Open original
- Comment list rendering `authorName` with `<app-avatar>`
- Comment form gated on `auth.has(Permissions.CommentWrite)`, with a
  "Sign in to comment" prompt for anonymous visitors
- Edit / delete affordances shown when the comment is the caller's own or they hold
  `comment.moderate`
- Comment like / dislike buttons (the endpoints work now; the leading-slash routing bug
  that made them 404 is fixed)

Note that `AuthorizedLayout.activeId` already maps `/article/*` onto the "All news" nav
entry, so the sidebar highlight needs no change.

### Realtime notifications (SignalR)

Integrate SignalR client once the NotificationService hub is up. Toast notification
when new news arrives, subtle unread counter in nav bar.

### Normalize the UI

The design system exists and the overall style holds together, but the screens do not
consistently follow it: font sizes vary where they should not, buttons and controls are
cramped or differently sized between screens, spacing is uneven. It reads as hand-assembled
rather than composed from the system.

The root cause is that several components hardcode values instead of using tokens, so the
system cannot enforce anything. `layout/components/settings-menu/settings-menu.scss` is the
clearest example: `font-size: 16px`, `19px`, `12px` and a literal
`font-family: 'IBM Plex Mono', monospace` where `var(--t-14)`, `var(--t-13)` and
`var(--font-mono)` belong — three different type sizes in one dropdown, none of them from
the scale.

**Scope:**
- Audit every `.scss` under `src/app/` for hardcoded `px` type sizes, colors and font
  families; replace with `--t-*`, `--s-*`, `--r-*`, `--font-*` and the theme variables.
  A grep for `font-size: \d+px` and `#[0-9a-f]{3,6}` finds most of them.
- Settle one control height (inputs are 36px in `_forms.scss`) and apply it to buttons,
  chips and dropdown triggers so rows line up.
- `common/_controls.scss` is still an empty placeholder — that is where the shared button
  and control geometry belongs, next to the form tokens.
- Pass over each screen against `docs/design/README.md`: PageShell + PageHeader usage,
  81px header, pager pinned to the page footer, `--s-6` / 30px body padding.
- Loading, empty and error states on every screen that fetches (several only have them on
  the feed).
- Mobile responsiveness — currently untested at any width below the desktop layout.

### Migrate custom components to PrimeNG

Replace handwritten shared components with PrimeNG equivalents where they exist, so we
stop maintaining bespoke versions of common UI primitives and inherit theming/a11y
from PrimeNG (Aura preset already configured).

**Scope:**
- Audit `src/app/shared/components/` and map each to its PrimeNG counterpart
  (e.g. `Avatar` → `p-avatar`, `Pager` → `p-paginator` — already used, `Sidebar` →
  `p-drawer` or keep custom, `Skeleton` → `p-skeleton`, `SearchBar` → `p-iconfield` +
  `p-inputtext`, `VoteButtons` → `p-button` group, `CategoryChips` → `p-chip`/`p-tag`,
  `ToastHost` → `p-toast` + `MessageService`).
- Decide which to keep custom (highly product-specific: `SourceBadge`,
  `ImagePlaceholder`, `EmptyState`/`ErrorState`/`ErrorBanner`, `NotFound`).
- Replace usage call-sites and remove obsolete components/SCSS.
- Verify theming (`--accent`, dark/light tokens) still applies via PrimeNG CSS layer.

## Testing

### NewsService integration tests

Add `NewsService.DAL.IntegrationTests` project mirroring CrawlerService integration
tests (Testcontainers for PostgreSQL). Cover `NewsStore`, `CommentStore` CRUD and
query paths.

### NotificationService tests

No tests exist for NotificationService today.

**Scope:**
- Unit tests for BLL (`WebhookDispatcher`, `WebhooksProcessor`, `WebhookTriggeredProcessor`,
  `WebhookDeliveryService`)
- DAL integration tests for `WebhookSubscriptionStore` (Testcontainers MongoDB)
- API tests for `WebhooksController`

## Infrastructure

### CI/CD pipeline

`.github/workflows/` is empty today. Set up automated build, test and (optionally) deploy.

**Scope:**
- `ci.yml` — on PR and push to `master`: restore → build all solutions → run unit
  tests → run integration tests (Testcontainers needs Docker runner)
- Build matrix: `NewsService.sln`, `CrawlerService.sln`, `NotificationService.sln`
- Angular build + unit tests (Vitest)
- Code coverage report upload
- `deploy.yml` (stretch) — build Docker images per service, push to registry,
  deploy via docker-compose on target host
- Branch protection: require green CI before merge

### MCP servers for the backend workspace

`.mcp.json` exists only under `src/client/NewsAggregatorClient/`, so its servers load
when working in the client directory and nowhere else. Backend work gets none of them.

**Scope:**
- Add an `.mcp.json` at the repository root so backend sessions get the shared servers:
  `context7` (library docs), `postgres` (`local-news` — now hosting both the `newsService`
  and `authService` schemas), `mongodb` (crawler + notification databases).
- Decide what to do about the duplication with the client file: either keep the root one
  generic and leave `angular` / `primeng` / `playwright` in the client, or hoist everything
  to the root and delete the client copy.
- Both files embed local dev connection strings, password included. Fine while it is
  localhost-only, but decide before this repository is ever shared — env-var indirection
  is the usual answer.
- Evaluate additions: a Seq server for querying logs, a GitHub server for issues and PRs.

### Finish the AI-development documentation setup

The AI-facing docs work, but they are half-configured and inconsistent between the two
CLAUDE.md files and the skills directory.

**Known gaps:**
- Only `create-project` uses the auto-discovered `.claude/skills/<name>/SKILL.md` layout.
  `create-migration`, `create-unit-test`, `up-to-date-tests` and `create-integration-test`
  are flat `.md` files, so they are **not invocable as slash commands** — they can only be
  referenced by path. Convert them, or state the choice deliberately.
- `create-integration-test` is still a placeholder that skips and logs.
- The client `CLAUDE.md` has an empty "Available Skills" section.
- No per-service CLAUDE.md files. The root file is getting long and mixes
  service-specific detail with cross-cutting architecture; splitting the service sections
  down into `src/server/<service>/CLAUDE.md` would keep each one loaded only when relevant.
- No `docs/` index tying together `TODO.md`, `Solution/Solution.md` and
  `src/client/NewsAggregatorClient/docs/design/README.md` — an agent has to already know
  they exist.
- Decide a review workflow: hooks (auto `dotnet format`, block commits touching
  appsettings with secrets) and whether specialized subagents (code-reviewer,
  test-writer) are worth configuring.
- New skill candidate: `add-source` — scaffold a NewsSource in the Mongo populator.
  (`new-service` from the old dev-experience list shipped as `create-project`.)

### Migrate logging from Seq to Grafana Loki

Currently all services ship logs to Seq (single-node, `http://localhost:5341`) via Serilog.
Seq is convenient for dev, but as the stack grows consider migrating to Grafana Loki +
Promtail for better integration with metrics/traces and long-term retention.

**Scope:**
- Run Loki + Promtail + Grafana in docker-compose alongside the existing stack
- Add `Serilog.Sinks.Grafana.Loki` to the shared `Logger` project or keep both sinks
  behind configuration
- Structured fields: `ServiceName`, `SourceId`, `CorrelationId` — propagate correlation
  id through RabbitMQ headers and restore on consumer side
- Dashboards: errors per service, crawl latency, news throughput
- Document connection settings and default credentials in `readme.md`
