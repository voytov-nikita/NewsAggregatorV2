import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-skeleton',
  templateUrl: './skeleton.html',
  styleUrl: './skeleton.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Skeleton {
  readonly width = input<number | string>('100%');
  readonly height = input<number | string>(14);
  readonly radius = input(4);
}
