import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DestroyRef } from '@angular/core';
import { ToastService } from '@shared/services';
import { Source } from '../../models/source.model';
import { SourcesService } from '../../services/sources.service';

const PAGE_SIZE = 6;

@Component({
  standalone: false,
  selector: 'app-sources-list',
  templateUrl: './sources-list.html',
  styleUrl: './sources-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SourcesList {
  private readonly toast = inject(ToastService);
  private readonly sourcesService = inject(SourcesService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly sources = signal<Source[]>([]);
  protected readonly loading = signal<boolean>(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly query = signal('');
  protected readonly currentPage = signal(1);
  protected readonly take = signal(PAGE_SIZE);
  protected readonly wizardOpen = signal(false);

  protected readonly filtered = computed(() => {
    const q = this.query().trim().toLowerCase();
    if (!q) return this.sources();
    return this.sources().filter(
      (s) => s.name.toLowerCase().includes(q) || s.url.toLowerCase().includes(q),
    );
  });

  protected readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.filtered().length / this.take())),
  );

  protected readonly paged = computed(() => {
    const start = (this.currentPage() - 1) * this.take();
    return this.filtered().slice(start, start + this.take());
  });

  constructor() {
    this.reload();
  }

  protected reload(): void {
    this.loading.set(true);
    this.loadError.set(null);
    this.sourcesService
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (list) => {
          this.sources.set(list ?? []);
          this.loading.set(false);
        },
        error: (err) => {
          this.loadError.set(err?.message ?? 'Failed to load sources');
          this.loading.set(false);
        },
      });
  }

  protected onSearch(value: string): void {
    this.query.set(value);
    this.currentPage.set(1);
  }

  protected onPageChange(p: number): void {
    this.currentPage.set(p);
  }

  protected onTakeChange(t: number): void {
    this.take.set(t);
    this.currentPage.set(1);
  }

  protected openWizard(): void {
    this.wizardOpen.set(true);
  }

  protected closeWizard(): void {
    this.wizardOpen.set(false);
  }

  protected onCreate(payload: Source): void {
    this.sourcesService
      .create(payload)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (created) => {
          this.sources.update((arr) => [created, ...arr]);
          this.wizardOpen.set(false);
          this.toast.success(`Source "${created.name}" added`);
        },
        error: (err) => {
          this.toast.error(err?.error ?? `Failed to add source`);
        },
      });
  }

  protected onToggleActive(s: Source): void {
    this.sourcesService
      .toggle(s.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (updated) => {
          this.sources.update((arr) => arr.map((x) => (x.id === updated.id ? updated : x)));
          this.toast.info(updated.enabled ? `Enabled "${updated.name}"` : `Paused "${updated.name}"`);
        },
        error: () => this.toast.error(`Failed to toggle "${s.name}"`),
      });
  }

  protected onRemove(s: Source): void {
    this.sourcesService
      .delete(s.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.sources.update((arr) => arr.filter((x) => x.id !== s.id));
          this.toast.success(`Source "${s.name}" removed`);
        },
        error: () => this.toast.error(`Failed to remove "${s.name}"`),
      });
  }

  protected onEdit(_: Source): void {
    // TODO: open wizard pre-filled for edit.
  }
}
