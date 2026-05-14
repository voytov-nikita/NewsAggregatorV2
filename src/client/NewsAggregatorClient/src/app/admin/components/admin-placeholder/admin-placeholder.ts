import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-admin-placeholder',
  templateUrl: './admin-placeholder.html',
  styleUrl: './admin-placeholder.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminPlaceholder {
  private readonly route = inject(ActivatedRoute);

  protected readonly section = toSignal(
    this.route.data.pipe(map((d) => d['section'] as string)),
    { initialValue: 'sources' },
  );
}
