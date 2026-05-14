# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
npm start          # Dev server at localhost:4200
```

All commands run from `src/client/NewsAggregatorClient/`.

## Architecture

**NgModule-based** Angular 21 app with lazy-loaded feature modules. Uses Angular Signals for component state plus **NgRx 21** (`@ngrx/store`, `@ngrx/effects`, `@ngrx/store-devtools`) for cross-module state (UI theme/sidebar, news feed). All components are `standalone: false` with `ChangeDetectionStrategy.OnPush`.

### Module Structure

- **AppModule** — Root module. Imports `LayoutModule`, configures NgRx store with `uiFeature` + `feedFeature` and effects (`UiEffects`, `FeedEffects`).
- **NewsModule** (lazy, `/feed`, `/article/:id`) — News list and detail views. Services: `NewsService`, `CommentsService`. Feed list reads/dispatches via NgRx; comments stay service-based.
- **SettingsModule** (lazy, `/settings/:section?`) — Single-page settings with sticky left nav and scroll-spy across 6 sections.
- **AdminModule** (lazy, `/admin/sources`, `/admin/stats`) — Sources list with 4-step wizard (URL+validate, crawler config, FieldMapper, review), and Statistics tab (KPIs + per-source bar chart). Mock data only.
- **SharedModule** — Reusable components: `Pager`, `Avatar`, `SourceBadge`, `ImagePlaceholder`, `Skeleton`, `VoteButtons`, `SearchBar`, `CategoryChips`, `Sidebar`. Imports/exports PrimeNG modules + `FormsModule` + `CommonModule`.
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

SCSS + PrimeNG 19 (Aura theme). CSS cascade layers: `@layer app-styles, primeng` (declared in `src/assets/css/layers.css`).

**SCSS structure:**
- `src/assets/scss/general/` — `_variables.scss`, `_mixins.scss`, `_base.scss`, `_primeng-overrides.scss`
- `src/assets/scss/common/` — `_masthead.scss`, `_breadcrumb.scss`, `_buttons.scss`, `_controls.scss`, `_forms.scss`, `_cards.scss`
- `src/styles.scss` — entry point, imports all partials via `@use`

**PrimeNG setup:** Configured in `app.module.ts` via `providePrimeNG()` with Aura preset, `darkModeSelector: '[data-theme="dark"]'`, CSS layer `primeng`. PrimeNG modules (Button, InputText, Textarea, ToggleSwitch, Popover, Menu, Paginator, Tag, Chip, Avatar, Breadcrumb, Tabs) are imported/exported through `SharedModule`.

**Theme:** Dark navy (default) + light variants via CSS custom properties on `:root[data-theme='dark'|'light']`. Fonts: IBM Plex Sans (body) + IBM Plex Mono (labels/code) loaded from Google Fonts in `index.html`. Accent: `#3b82f6`. Per-category badge colors via `oklch()` hue rotations. Component files use `.scss` extension.

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
