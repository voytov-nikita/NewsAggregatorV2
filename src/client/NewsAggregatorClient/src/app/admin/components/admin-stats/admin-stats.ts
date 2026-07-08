import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Source } from '../../models/source.model';
import { SourcesService } from '../../services/sources.service';

interface Kpi {
  label: string;
  value: string;
  sub: string;
}

@Component({
  standalone: false,
  selector: 'app-admin-stats',
  templateUrl: './admin-stats.html',
  styleUrl: './admin-stats.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminStats {
  private readonly sourcesService = inject(SourcesService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly sources = signal<Source[]>([]);
  protected readonly loading = signal<boolean>(true);

  protected readonly rssCount = computed(() => this.sources().filter((s) => s.type === 'Feed').length);
  protected readonly htmlCount = computed(() => this.sources().filter((s) => s.type === 'Html').length);
  protected readonly activeCount = computed(() => this.sources().filter((s) => s.enabled).length);

  protected readonly totalArticles = computed(() =>
    this.sources().reduce((acc, s) => acc + (s.articlesCount ?? 0), 0),
  );

  protected readonly avgSuccess = computed(() => {
    const withResult = this.sources().filter((s) => s.lastCrawlSuccess !== null && s.lastCrawlSuccess !== undefined);
    if (withResult.length === 0) return null;
    const ok = withResult.filter((s) => s.lastCrawlSuccess === true).length;
    return Math.round((ok / withResult.length) * 100);
  });

  protected readonly kpis = computed<Kpi[]>(() => {
    const avg = this.avgSuccess();
    return [
      {
        label: 'Total articles',
        value: this.totalArticles().toLocaleString(),
        sub: `across ${this.sources().length} sources`,
      },
      {
        label: 'Last-crawl success',
        value: avg === null ? '—' : `${avg}%`,
        sub: avg === null ? 'awaiting first crawl' : 'across sources with at least one crawl',
      },
      {
        label: 'Active sources',
        value: `${this.activeCount()} / ${this.sources().length}`,
        sub: 'currently crawling',
      },
      {
        label: 'RSS / HTML',
        value: `${this.rssCount()} · ${this.htmlCount()}`,
        sub: 'feed type split',
      },
    ];
  });

  protected readonly maxArticles = computed(() =>
    Math.max(1, ...this.sources().map((s) => s.articlesCount ?? 0)),
  );

  protected readonly chartRows = computed(() =>
    [...this.sources()]
      .sort((a, b) => (b.articlesCount ?? 0) - (a.articlesCount ?? 0))
      .map((s) => ({
        ...s,
        widthPct: Math.round(((s.articlesCount ?? 0) / this.maxArticles()) * 100),
      })),
  );

  constructor() {
    this.sourcesService
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (list) => {
          this.sources.set(list ?? []);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  protected lastCrawlLabel(s: Source): string {
    if (!s.lastCrawlAt) return 'never';
    const t = new Date(s.lastCrawlAt).getTime();
    const diffMs = Date.now() - t;
    const min = Math.floor(diffMs / 60_000);
    if (min < 1) return 'just now';
    if (min < 60) return `${min}m ago`;
    const h = Math.floor(min / 60);
    if (h < 24) return `${h}h ago`;
    const d = Math.floor(h / 24);
    return `${d}d ago`;
  }

  protected successClass(s: Source): string {
    if (s.lastCrawlSuccess === true) return 'ok';
    if (s.lastCrawlSuccess === false) return 'bad';
    return 'muted';
  }

  protected successLabel(s: Source): string {
    if (s.lastCrawlSuccess === true) return 'ok';
    if (s.lastCrawlSuccess === false) return 'failed';
    return '—';
  }
}
