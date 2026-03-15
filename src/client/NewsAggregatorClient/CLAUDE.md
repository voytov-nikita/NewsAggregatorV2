# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
npm start          # Dev server at localhost:4200
```

All commands run from `src/client/NewsAggregatorClient/`.

## Architecture

**NgModule-based** Angular 21 app with lazy-loaded feature modules. Uses Angular Signals for component state (not NgRx or other state libraries).

### Module Structure

- **AppModule** — Root module. Imports `LayoutModule`, `SharedModule`, sets up routing.
- **NewsModule** (lazy) — News list and detail views. Own services: `NewsService`, `CommentsService`.
- **SettingsModule** (lazy) — Settings page.
- **SharedModule** — Reusable components (`Pager`), models, enums, `NavigationService`.
- **LayoutModule** — `AuthorizedLayout` component (page shell with navigation).

### Path Aliases (tsconfig.json)

`@shared/*`, `@news/*`, `@layout/*`, `@settings/*` map to `src/app/<module>/*`.

### API Communication

Services use `HttpClient` directly with `providedIn: 'root'`. Base URL is hardcoded to `https://localhost:7300/api/v1/news` (NewsService backend). No interceptors or global error handling currently.

### Component Patterns

- Local state via `signal()`, `computed()`, reactive inputs via `input()` / `input.required()`, outputs via `output()`.
- RxJS: `debounceTime(300)` for search, `switchMap()` for flattening, `takeUntilDestroyed()` for cleanup, `finalize()` for loading states.
- Component files use `.ts` extension (not `.component.ts`) — e.g., `list.ts`, `detail.ts`.

### Testing

Vitest 4.0.8 + jsdom. Tests use Angular `TestBed`. Test files: `*.spec.ts`.

### Styling

CSS custom properties for theming. Fonts: Crimson Pro (serif headings), Libre Franklin (sans-serif body). Accent color: `#d4574d`.

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
