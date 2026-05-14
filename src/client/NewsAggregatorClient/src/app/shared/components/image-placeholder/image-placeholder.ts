import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-image-placeholder',
  templateUrl: './image-placeholder.html',
  styleUrl: './image-placeholder.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ImagePlaceholder {
  readonly width = input<number | string>('100%');
  readonly height = input<number | string>(120);
  readonly label = input('image');
}
