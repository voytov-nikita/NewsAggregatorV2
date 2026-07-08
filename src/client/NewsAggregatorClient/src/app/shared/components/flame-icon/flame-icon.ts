import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/**
 * Minimal outline flame glyph (uses `currentColor`), shaped to sit alongside
 * the unicode glyphs in the sidebar (★, ⊞, ⊕) rather than the heavy filled
 * emoji 🔥.
 */
@Component({
  standalone: false,
  selector: 'app-flame-icon',
  templateUrl: './flame-icon.html',
  styleUrl: './flame-icon.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FlameIcon {
  readonly size = input<number>(16);
}
