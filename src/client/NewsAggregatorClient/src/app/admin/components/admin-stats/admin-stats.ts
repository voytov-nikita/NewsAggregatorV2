import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';
import { MOCK_SOURCES } from '../../data/admin-mock';
import { Source } from '../../models/source.model';

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
  protected readonly sources = signal<Source[]>(MOCK_SOURCES);

  protected readonly totalArticles = computed(() =>
    this.sources().reduce((acc, s) => acc + s.articles, 0),
  );

  protected readonly avgSuccess = computed(() => {
    const arr = this.sources();
    if (!arr.length) return 0;
    return Math.round(arr.reduce((acc, s) => acc + s.success, 0) / arr.length);
  });

  protected readonly rssCount = computed(() => this.sources().filter((s) => s.type === 'RSS').length);
  protected readonly htmlCount = computed(() => this.sources().filter((s) => s.type === 'HTML').length);
  protected readonly activeCount = computed(() => this.sources().filter((s) => s.active).length);

  protected readonly kpis = computed<Kpi[]>(() => [
    {
      label: 'Total articles',
      value: this.totalArticles().toLocaleString(),
      sub: `across ${this.sources().length} sources`,
    },
    {
      label: 'Success rate',
      value: `${this.avgSuccess()}%`,
      sub: 'last 24h average',
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
  ]);

  protected readonly maxArticles = computed(() => Math.max(1, ...this.sources().map((s) => s.articles)));

  protected readonly chartRows = computed(() =>
    [...this.sources()]
      .sort((a, b) => b.articles - a.articles)
      .map((s) => ({
        ...s,
        widthPct: Math.round((s.articles / this.maxArticles()) * 100),
      })),
  );

  protected successClass(success: number): string {
    if (success >= 95) return 'ok';
    if (success >= 90) return 'warn';
    return 'bad';
  }
}
