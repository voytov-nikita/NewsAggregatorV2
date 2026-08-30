# Design system

Local snapshot of the design system the Angular client follows. Source of truth
for **values** is `src/assets/scss/general/_variables.scss`; this folder is the
source of truth for **intent and patterns**.

The `Design System.html` file is a verbatim snapshot of the Claude-design
prototype's `prototype/Design System.html`. Keep it for reference, but never
import it into the bundle — it stays in `docs/` outside `src/`.

## When to read what

| Question                                       | Where to look |
|------------------------------------------------|---------------|
| What's the exact value of `--t-22` / `--s-5`?  | `src/assets/scss/general/_variables.scss` |
| What does a primary button look like / states? | `Design System.html` (sections "Buttons", "Inputs", "States") |
| Should I build a custom header or reuse one?   | `Use shared components` below |
| What hue / OKLCH for category X?               | `src/app/shared/data/news-catalog.ts` + `_variables.scss` |

## Tokens at a glance

CSS custom properties exposed by `_variables.scss`:

- **Type scale** (`--t-9` … `--t-48`): 9 / 10 / 11 / 12 / 13 / 14 / 16 / 18 / 22 / 28 / 36 / 48 px
- **Spacing** (`--s-1` … `--s-10`): 4 / 6 / 8 / 12 / 16 / 20 / 24 / 32 / 48 / 64 px
- **Radii** (`--r-xs` … `--r-pill`): 4 / 6 / 8 / 10 / 14 / 100 px
- **Colors (theme-aware):** `--bg`, `--surface-1/2/3`, `--border`, `--text-primary/secondary/muted`, `--input-bg`, `--overlay`
- **Shadows (theme-aware):** `--shadow-1` (raised), `--shadow-2` (popover), `--shadow-3` / `--shadow-modal`
- **Semantic (shared across themes):** `--success`, `--warning`, `--danger`, `--info` + `-bg` variants (OKLCH-based)
- **Accent** (purple by default, palette in `_variables.scss` comment):
  - blue   `#3b82f6` / `rgba(59,130,246,0.10)`
  - purple `#8b5cf6` / `rgba(139,92,246,0.10)`  ← **active**
  - teal   `#14b8a6` / `rgba(20,184,166,0.10)`

## Mixins (`_mixins.scss`)

- `@mixin mono` — IBM Plex Mono
- `@mixin uppercase-label` — mono 10px, uppercase, letter-spacing 0.08em (eyebrows / meta)
- `@mixin card` — surface-1 + border + radius `--r-md`
- `@mixin focus-ring` — accent border + 3px accent-bg shadow
- `@mixin category-chip($hue)` / `@mixin category-from-var` — OKLCH category badge

## Use shared components — do not roll your own

Before writing UI markup, check if there's a shared component for it. Adding a
new ad-hoc page header / button / pager fragments the design and makes the
purple-swap / theme-swap painful next time.

| Need               | Shared component / token         |
|--------------------|----------------------------------|
| Page title bar     | `<app-page-header>` (title + leading/meta/actions slots; height 81, 23/700) |
| Page layout (header + scroll + sticky pager) | `<app-page-shell>` (slots `pageHeader`, default, `pageFooter`) |
| Pagination         | `<app-pager>` inside `[pageFooter]` slot of PageShell |
| Search input       | `<app-search-bar>` |
| Category badge     | `<app-source-badge [category]="…">` (hides Uncategorized automatically) |
| Empty / error      | `<app-empty-state>`, `<app-error-state>`, `<app-error-banner>` |
| Loading            | `<app-skeleton>` |
| Toast              | `<app-toast-host>` + `ToastService` |
| Sidebar nav        | `<app-sidebar>` |
| Form field         | `.field` + `.field__label` / `__hint` / `__error` (`common/_forms.scss`) |
| Form-level error   | `.form-error` |

## Page anatomy

Every top-level screen looks like this:

```html
<app-page-shell>
  <app-page-header pageHeader title="Page name">
    <button pageHeaderLeading>…</button>            <!-- optional: back / filter toggle / icon -->
    <span pageHeaderMeta>42 items</span>            <!-- optional: counter / subtitle -->
    <div pageHeaderActions>…</div>                  <!-- optional: sort / tabs / CTA -->
  </app-page-header>

  <div class="body">
    <!-- scrollable content; constrain max-width, padding: var(--s-6) 30px -->
  </div>

  <div pageFooter>                                  <!-- optional: only if there's a pager -->
    <app-pager …></app-pager>
  </div>
</app-page-shell>
```

Component SCSS shape:

```scss
:host {
  display: flex;
  flex: 1;
  min-height: 0;
  flex-direction: column;
}
.body {
  padding: var(--s-6) 30px;
  /* No max-width / margin: 0 auto. The sidebar already constrains the
     main area; centering the body inside it leaves dead gutters on wide
     screens — see prototype "AllNewsScreen" for the canonical layout. */
}
```

The page header is **always 81px tall** (`_page-header.scss`); pager is
**always pinned to the bottom of the page**, never to the bottom of the table.

## Forms

Form vocabulary lives in `common/_forms.scss` — `.field` with `__label`, `__hint`,
`__error`, the `.form-error` block for a server-level failure, and `.auth-form` for
the sign-in / sign-up stack. Inputs come from PrimeNG (`pInputText`); the partial only
sets the shared geometry (36px height, `--r-sm` radius, `@include focus-ring`), so
theming stays PrimeNG's job.

Do not restyle inputs inside a feature's own SCSS. If a form needs something the
partial does not have, add it there — the second form should inherit it.

## Exception to the PageShell rule: AuthShell

`AuthShell` (`/auth/login`, `/auth/register`) is a centered card and is deliberately
**not** wrapped in `<app-page-shell>`. The "every top-level screen is wrapped in
PageShell" rule covers screens rendered *inside* the application frame — sidebar, page
header, pinned footer. The sign-in screens render before that frame exists and have no
navigation to sit in, so PageShell would only add an empty 81px header bar.

It is the only exception. A new screen that lives inside `AuthorizedLayout` still
follows the rule above.

## Density / accent / theme — keep them token-driven

If a value needs to change with theme or accent, it must be a CSS custom
property. No hardcoded `#8b5cf6` in component styles — use `var(--accent)`. Same
for `var(--surface-1)`, `var(--text-primary)` and friends.

The accent itself is a single token: changing `_variables.scss` `--accent` /
`--accent-bg` re-skins the entire app (sidebar active state, filter toggle,
follow buttons, focus rings, source badges, pager dots, etc).
