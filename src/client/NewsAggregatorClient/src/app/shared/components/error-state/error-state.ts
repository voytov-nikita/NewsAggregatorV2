import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-error-state',
  templateUrl: './error-state.html',
  styleUrl: './error-state.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ErrorState {
  readonly title = input<string>("Couldn't load data");
  readonly description = input<string>('Network error. Check your connection and try again.');
  readonly retryLabel = input<string>('Try again');
  readonly retry = output<void>();
}
