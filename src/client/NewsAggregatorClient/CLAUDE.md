# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
npm start          # Dev server at localhost:4200
```

All commands run from `src/client/NewsAggregatorClient/`.

## MCP Servers

Project-scoped MCP servers are committed in `.mcp.json` (loaded by Claude Code): `angular` (`@angular/cli mcp`), `primeng` (`@primeng/mcp`), `context7` (`@upstash/context7-mcp`), `playwright` (`@playwright/mcp` — browser automation), `postgres` (NewsService DB), `mongodb` (crawler/notification DBs). DB servers use local dev connection strings.

## Architecture

**NgModule-based** Angular 21 app with lazy-loaded feature modules. Uses Angular Signals for component state plus **NgRx 21** (`@ngrx/store`, `@ngrx/effects`, `@ngrx/store-devtools`) for cross-module state (UI theme/sidebar, news feed). All components are `standalone: false` with `ChangeDetectionStrategy.OnPush`.

### Module Structure

- **AppModule** — Root module. Imports `LayoutModule`, configures NgRx store with `uiFeature` + `feedFeature` and effects (`UiEffects`, `FeedEffects`).
- **NewsModule** (lazy, `/allnews`, `/article/:id`; `/feed` kept as redirect alias) — News list and detail views. Services: `NewsService`, `CommentsService`. Feed list reads/dispatches via NgRx; comments stay service-based.
- **ForYouModule** (lazy, `/foryou`) — Personalized feed; shows category-picker onboarding when `FollowsService` is empty, otherwise ranks articles by followed categories (+3) and sources (+4) with "because you follow …" reasons.
- **TrendingModule** (lazy, `/trending`) — Live trending list with hour/day/week range tabs, flame icon, live-readers indicator, and "spiking" badge.
- **SubscriptionsModule** (lazy, `/subscriptions`) — Manage follows across 3 tabs (Categories / Sources / Authors) with search and Follow/Following toggle.
- **SavedModule** (lazy, `/saved`) — Bookmarked articles grouped by Date / Category / None, backed by `SavedArticlesService` (localStorage).
- **SettingsModule** (lazy, `/settings/:section?`) — Single-page settings with sticky left nav and scroll-spy across 6 sections.
- **AdminModule** (lazy, `/admin/sources`, `/admin/stats`) — Sources list with 4-step wizard (URL+validate, crawler config, FieldMapper, review), and Statistics tab (KPIs + per-source breakdown table + bar chart). Mock data only.
- **SharedModule** — Reusable components: `Pager`, `Avatar`, `SourceBadge`, `ImagePlaceholder`, `Skeleton`, `VoteButtons`, `SearchBar`, `CategoryChips`, `Sidebar`, `ToastHost`, `EmptyState`, `ErrorState`, `ErrorBanner`, `NotFound`, **`PageHeader`** (universal 81px page top: title + leading/meta/actions slots), **`PageShell`** (header + scroll content + sticky footer for pagers). Imports/exports PrimeNG modules + `FormsModule` + `CommonModule`. Shared services: `NavigationService`, `ThemeService`, `ToastService`, `SavedArticlesService` (localStorage `na-saved`), `FollowsService` (localStorage `na-follows`). Shared data: `news-catalog.ts` (`CATEGORIES`, `CATEGORY_HUES`, `categoryFor`, `readTimeFor`).
- **LayoutModule** — `AuthorizedLayout` (sidebar + main + settings-menu shell), `SettingsMenu` (top-right ⚙ dropdown).

### Path Aliases (tsconfig.json)

`@shared/*`, `@news/*`, `@layout/*`, `@settings/*` map to `src/app/<module>/*`.

### API Communication

Services use `HttpClient` with `providedIn: 'root'`. Base URL comes from `src/environments/environment.ts` (`newsApiBaseUrl`); production replacement is configured in `angular.json` `fileReplacements` to `environment.prod.ts`. Feed errors are formatted in `FeedEffects` and surfaced via `feedFeature.selectError` rendered as a dismissable banner above the feed.

### State (NgRx)

Slices live under `src/app/store/`:
- `ui/` — theme (`dark`/`light`) + sidebar collapsed; persisted to localStorage via `UiEffects`; applies `data-theme` attribute to `documentElement`.
- `feed/` — articles, isLoading, error, search/sort/category/page/pageSize. `FeedEffects.load$` calls `NewsService.getMany`. NewsList component is fully NgRx-driven (`toSignal` over selectors, dispatches actions).

### Component Patterns

- Local state via `signal()`, `computed()`, reactive inputs via `input()` / `input.required()`, outputs via `output()`.
- RxJS: `debounceTime(300)` for search, `switchMap()` for flattening, `takeUntilDestroyed()` for cleanup, `finalize()` for loading states.
- Component files use `.ts` extension (not `.component.ts`) — e.g., `list.ts`, `detail.ts`.

### Testing

Vitest 4.0.8 + jsdom. Tests use Angular `TestBed`. Test files: `*.spec.ts`.

### Styling

SCSS + PrimeNG 21 (Aura theme). CSS cascade layers: `@layer app-styles, primeng` (declared in `src/assets/css/layers.css`).

**SCSS structure:**
- `src/assets/scss/general/` — `_variables.scss`, `_mixins.scss`, `_base.scss`, `_primeng-overrides.scss`
- `src/assets/scss/common/` — `_masthead.scss`, `_breadcrumb.scss`, `_buttons.scss`, `_controls.scss`, `_forms.scss`, `_cards.scss`
- `src/styles.scss` — entry point, imports all partials via `@use`

**PrimeNG setup:** Configured in `app.module.ts` via `providePrimeNG()` with Aura preset, `darkModeSelector: '[data-theme="dark"]'`, CSS layer `primeng`. PrimeNG modules (Button, InputText, Textarea, ToggleSwitch, Popover, Menu, Paginator, Tag, Chip, Avatar, Breadcrumb, Tabs) are imported/exported through `SharedModule`.

**Theme:** Dark navy (default) + light variants via CSS custom properties on `:root[data-theme='dark'|'light']`. Fonts: IBM Plex Sans (body) + IBM Plex Mono (labels/code) loaded from Google Fonts in `index.html`. Accent (currently): `#8b5cf6` (purple). Per-category badge colors via `oklch()` hue rotations. Component files use `.scss` extension.

### Design system — READ BEFORE TOUCHING UI

The app follows a documented design system. Authoritative sources, in order:

1. **Values** — `src/assets/scss/general/_variables.scss` (type / spacing / radii / shadows / accent palette / theme colors). Never hardcode `#8b5cf6` or `16px` — use `var(--accent)` / `var(--s-5)` etc.
2. **Patterns + rules** — [`docs/design/README.md`](docs/design/README.md): which shared component to reuse for which need, page anatomy, accent / theme rules, mixin catalogue.
3. **Full reference snapshot** — [`docs/design/Design System.html`](docs/design/) (verbatim from the Claude-design prototype; reference only — never imported into the bundle).

**Rules that MUST be followed without being asked:**

- Every top-level screen is wrapped in `<app-page-shell>` with `<app-page-header pageHeader title="…">` for the title bar. Do not invent a per-screen `.page-head`.
- Pagers go inside the `[pageFooter]` slot of `<app-page-shell>` so they pin to the bottom of the page, not the bottom of the list.
- Before writing a custom button / badge / search box / loader, check `src/app/shared/components/` — there's likely an existing one. The shared component catalogue is in [`docs/design/README.md`](docs/design/README.md).
- Theme-/accent-sensitive values must be CSS custom properties so theme toggling and accent swaps stay one-line changes.

## Backend

Microservices backend on .NET 9 / C#. Three services communicate via RabbitMQ:

- **NewsService** — Main API for news and comments. PostgreSQL + EF Core.
- **CrawlerService** — Scheduled RSS crawler (Hangfire). MongoDB.
- **NotificationService** — Webhook/notification delivery. MongoDB.

The client communicates with NewsService API (`https://localhost:7300/api/v1/news`).

See full details: [`../../CLAUDE.md`](../../../CLAUDE.md)

## Available Skills


# Important — MUST follow without being asked

ALWAYS update this CLAUDE.md immediately after any change that affects architecture, models, commands, environments, or deployment — without waiting for the user to ask. Keep additions concise and to the point: one fact per line, no redundant context.

If asked to review CLAUDE.md for accuracy, read the current state of the project and update any outdated information.
