import { ChangeDetectionStrategy, Component, DestroyRef, effect, inject } from '@angular/core';
import { ToastService, type Toast } from '../../services/toast.service';

const AUTO_HIDE_MS = 4200;
const HOVER_RESUME_MS = 2200;

@Component({
  standalone: false,
  selector: 'app-toast-host',
  templateUrl: './toast-host.html',
  styleUrl: './toast-host.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastHost {
  protected readonly toastService = inject(ToastService);
  protected readonly toasts = this.toastService.toasts;

  private readonly timers = new Map<number, ReturnType<typeof setTimeout>>();

  constructor() {
    effect(() => {
      const current = this.toasts();
      const liveIds = new Set(current.map((t) => t.id));
      for (const id of [...this.timers.keys()]) {
        if (!liveIds.has(id)) {
          clearTimeout(this.timers.get(id));
          this.timers.delete(id);
        }
      }
      for (const t of current) {
        if (!this.timers.has(t.id)) this.schedule(t.id, AUTO_HIDE_MS);
      }
    });

    inject(DestroyRef).onDestroy(() => {
      for (const timer of this.timers.values()) clearTimeout(timer);
      this.timers.clear();
    });
  }

  protected pause(id: number) {
    const timer = this.timers.get(id);
    if (timer) {
      clearTimeout(timer);
      this.timers.set(id, undefined as unknown as ReturnType<typeof setTimeout>);
    }
  }

  protected resume(id: number) {
    this.schedule(id, HOVER_RESUME_MS);
  }

  protected close(id: number) {
    this.toastService.dismiss(id);
  }

  protected iconFor(type: Toast['type']) {
    return type === 'success' ? '✓' : type === 'info' ? 'i' : '!';
  }

  private schedule(id: number, delay: number) {
    const existing = this.timers.get(id);
    if (existing) clearTimeout(existing);
    this.timers.set(
      id,
      setTimeout(() => this.close(id), delay),
    );
  }
}
