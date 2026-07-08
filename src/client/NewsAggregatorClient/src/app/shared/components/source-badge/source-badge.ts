import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

const CATEGORY_HUES: Record<string, number> = {
  Technology: 220,
  Architecture: 160,
  Backend: 30,
  Frontend: 280,
  Database: 200,
  DevOps: 120,
  RealTime: 10,
  State: 250,
};

const CATEGORY_LABELS: Record<string, string> = {
  RealTime: 'Real-time',
};

@Component({
  standalone: false,
  selector: 'app-source-badge',
  templateUrl: './source-badge.html',
  styleUrl: './source-badge.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SourceBadge {
  readonly category = input.required<string>();

  protected readonly hue = computed(() => CATEGORY_HUES[this.category()] ?? 220);
  protected readonly label = computed(() => CATEGORY_LABELS[this.category()] ?? this.category());
}
