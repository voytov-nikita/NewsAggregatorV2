import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/**
 * Universal page header — applied to every top-level screen.
 * Layout (matches prototype/na-shared.jsx page-header pattern):
 *   ┌────────────────────────────────────────────────────────────┐
 *   │  [leading] H1 [meta]                          [actions]    │
 *   └────────────────────────────────────────────────────────────┘
 * Slots:
 *   <ng-content select="[pageHeaderLeading]"> — left of the title (back button, filter toggle).
 *   <ng-content select="[pageHeaderMeta]">    — inline after the title (e.g. "42 articles" counter).
 *   <ng-content select="[pageHeaderActions]"> — right side (sort select, group-by tabs, CTA buttons).
 */
@Component({
  standalone: false,
  selector: 'app-page-header',
  templateUrl: './page-header.html',
  styleUrl: './page-header.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageHeader {
  readonly title = input.required<string>();
}
