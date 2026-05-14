import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-avatar',
  templateUrl: './avatar.html',
  styleUrl: './avatar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Avatar {
  readonly name = input.required<string>();
  readonly size = input(32);

  protected readonly initials = computed(() =>
    this.name()
      .split(/[_\s.]/)
      .map((p) => p[0] ?? '')
      .join('')
      .toUpperCase()
      .slice(0, 2),
  );

  protected readonly hue = computed(() => {
    const n = this.name();
    let sum = 0;
    for (let i = 0; i < n.length; i++) sum += n.charCodeAt(i);
    return sum % 360;
  });
}
