import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';
import { MOCK_SOURCES } from '../../data/admin-mock';
import { Source } from '../../models/source.model';

const PAGE_SIZE = 6;

@Component({
  standalone: false,
  selector: 'app-sources-list',
  templateUrl: './sources-list.html',
  styleUrl: './sources-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SourcesList {
  protected readonly sources = signal<Source[]>(MOCK_SOURCES);
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

  protected onCreate(payload: Omit<Source, 'id' | 'articles' | 'success' | 'avgDelay' | 'lastCrawl'>): void {
    const id = Math.max(0, ...this.sources().map((s) => s.id)) + 1;
    const next: Source = {
      ...payload,
      id,
      articles: 0,
      success: 100,
      avgDelay: 0,
      lastCrawl: 'just now',
    };
    this.sources.update((arr) => [next, ...arr]);
    this.wizardOpen.set(false);
  }

  protected onToggleActive(s: Source): void {
    this.sources.update((arr) =>
      arr.map((x) => (x.id === s.id ? { ...x, active: !x.active } : x)),
    );
  }

  protected onRemove(s: Source): void {
    this.sources.update((arr) => arr.filter((x) => x.id !== s.id));
  }

  protected onEdit(_: Source): void {
    // Placeholder: opens wizard pre-filled in future iteration.
  }
}
