import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-empty-state',
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmptyState {
  readonly glyph = input<string>('◇');
  readonly title = input.required<string>();
  readonly description = input<string>('');
}
