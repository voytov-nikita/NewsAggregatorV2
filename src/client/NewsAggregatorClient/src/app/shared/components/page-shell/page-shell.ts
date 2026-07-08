import { ChangeDetectionStrategy, Component } from '@angular/core';

/**
 * Top-level page container with three stacked regions:
 *   1. Header  — always visible at top (use <app-page-header> here).
 *   2. Content — scrollable, fills remaining space.
 *   3. Footer  — pinned to the bottom of the page, used for the pager
 *                (and only rendered when a pager actually exists on the page).
 *
 * Slots: <ng-content select="[pageHeader]"|"[pageFooter]"> + default content.
 */
@Component({
  standalone: false,
  selector: 'app-page-shell',
  templateUrl: './page-shell.html',
  styleUrl: './page-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageShell {}
