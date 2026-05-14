import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { Source } from '../../models/source.model';

@Component({
  standalone: false,
  selector: 'app-source-card',
  templateUrl: './source-card.html',
  styleUrl: './source-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SourceCard {
  readonly source = input.required<Source>();
  readonly edit = output<Source>();
  readonly toggleActive = output<Source>();
  readonly remove = output<Source>();
}
