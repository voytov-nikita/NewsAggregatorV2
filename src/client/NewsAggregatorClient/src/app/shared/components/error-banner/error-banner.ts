import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-error-banner',
  templateUrl: './error-banner.html',
  styleUrl: './error-banner.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ErrorBanner {
  readonly title = input.required<string>();
  readonly description = input<string>('');
  readonly showRetry = input<boolean>(false);
  readonly showClose = input<boolean>(true);
  readonly retry = output<void>();
  readonly closed = output<void>();
}
