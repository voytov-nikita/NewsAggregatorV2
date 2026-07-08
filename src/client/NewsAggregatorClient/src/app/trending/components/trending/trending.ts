import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { NewsService } from '../../../news/services/news.service';
import { NewsResponse } from '@shared/models';
import { NewsOrderField, OrderDirection } from '@shared/enums';
import { NavigationService, SavedArticlesService } from '@shared/services';
import { categoryFor, readTimeFor } from '@shared/data/news-catalog';

type TimeRange = 'hour' | 'day' | 'week';

interface TrendingEntry {
  article: NewsResponse;
  views: number;
  readers: number;
  spiking: boolean;
}

const RANGE_TABS: { id: TimeRange; label: string; unit: string }[] = [
  { id: 'hour', label: '1 hour',   unit: '/hr'  },
  { id: 'day',  label: '24 hours', unit: '/day' },
  { id: 'week', label: '7 days',   unit: '/wk'  },
];

@Component({
  standalone: false,
  selector: 'app-trending',
  templateUrl: './trending.html',
  styleUrl: './trending.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Trending implements OnInit {
  private readonly newsService = inject(NewsService);
  private readonly savedArticles = inject(SavedArticlesService);
  protected readonly navigation = inject(NavigationService);

  protected readonly ranges = RANGE_TABS;

  protected readonly range = signal<TimeRange>('hour');
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly articles = signal<NewsResponse[]>([]);

  protected readonly rangeUnit = computed(
    () => RANGE_TABS.find((r) => r.id === this.range())?.unit ?? '',
  );

  protected readonly entries = computed<TrendingEntry[]>(() => {
    const r = this.range();
    return this.articles()
      .map((article) => {
        const seed = article.id * 137;
        const baseLikes = article.likes ?? 0;
        const baseComments = article.commentsCount ?? 0;
        const ageHours = Math.max(0.5, (Date.now() - new Date(article.publishDate).getTime()) / 3_600_000);
        // Deterministic synthetic views per range — mirrors the prototype's getViews helper.
        const factor = r === 'hour' ? 80 : r === 'day' ? 900 : 4500;
        const views = Math.round((factor + (seed % 4000)) / Math.pow(ageHours + 1, 0.3));
        return {
          article,
          views: views + baseLikes * 5 + baseComments * 10,
          readers: 40 + (seed % 760),
          spiking: r !== 'week' && (article.id * 13) % 4 === 0,
        };
      })
      .sort((a, b) => b.views - a.views);
  });

  protected readonly heroEntries = computed(() => this.entries().slice(0, 4));
  protected readonly compactEntries = computed(() => this.entries().slice(4));

  public async ngOnInit(): Promise<void> {
    await this.load();
  }

  protected async load(): Promise<void> {
    this.isLoading.set(true);
    this.error.set(null);
    try {
      const page = await firstValueFrom(
        this.newsService.getMany({
          take: 60,
          offset: 0,
          orderBy: NewsOrderField.PublishDate,
          orderDirection: OrderDirection.Descending,
        }),
      );
      this.articles.set(page.items);
    } catch (e) {
      this.error.set(e instanceof Error ? e.message : 'Failed to load trending.');
    } finally {
      this.isLoading.set(false);
    }
  }

  protected setRange(value: TimeRange): void {
    this.range.set(value);
  }

  protected open(article: NewsResponse): void {
    this.navigation.openOriginal(article);
  }

  protected toggleSaved(article: NewsResponse, event: Event): void {
    event.stopPropagation();
    this.savedArticles.toggle(article.id);
  }

  protected isSaved(id: number): boolean {
    return this.savedArticles.has(id);
  }

  protected categoryFor = categoryFor;
  protected readTimeFor = readTimeFor;

  protected formatCount(n: number): string {
    if (n >= 1_000_000) return (n / 1_000_000).toFixed(1) + 'M';
    if (n >= 1_000) return (n / 1_000).toFixed(1) + 'K';
    return n.toString();
  }

  protected formatRelativeTime(iso: string): string {
    const diff = Date.now() - new Date(iso).getTime();
    const h = Math.floor(diff / 3_600_000);
    if (h < 1) return `${Math.max(1, Math.floor(diff / 60_000))}m ago`;
    if (h < 24) return `${h}h ago`;
    return `${Math.floor(h / 24)}d ago`;
  }
}
