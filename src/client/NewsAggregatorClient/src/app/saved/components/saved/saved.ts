import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { NewsService } from '../../../news/services/news.service';
import { NewsResponse } from '@shared/models';
import { NewsOrderField, OrderDirection } from '@shared/enums';
import { NavigationService, SavedArticlesService, ToastService } from '@shared/services';
import { categoryFor, readTimeFor } from '../../../shared/data/news-catalog';

type GroupBy = 'date' | 'category' | 'none';

interface Group {
  key: string;
  label: string;
  items: NewsResponse[];
}

@Component({
  standalone: false,
  selector: 'app-saved',
  templateUrl: './saved.html',
  styleUrl: './saved.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Saved implements OnInit {
  private readonly newsService = inject(NewsService);
  private readonly savedArticles = inject(SavedArticlesService);
  private readonly toast = inject(ToastService);
  protected readonly navigation = inject(NavigationService);

  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly allArticles = signal<NewsResponse[]>([]);
  protected readonly groupBy = signal<GroupBy>('date');

  protected readonly savedIds = this.savedArticles.ids;

  protected readonly saved = computed(() => {
    const ids = this.savedIds();
    return this.allArticles().filter((a) => ids.has(a.id));
  });

  protected readonly groups = computed<Group[]>(() => {
    const items = this.saved();
    if (!items.length) return [];

    switch (this.groupBy()) {
      case 'none':
        return [{ key: 'all', label: 'All saved', items }];
      case 'category': {
        const byCat = new Map<string, NewsResponse[]>();
        items.forEach((a) => {
          const cat = categoryFor(a);
          if (!byCat.has(cat)) byCat.set(cat, []);
          byCat.get(cat)!.push(a);
        });
        return [...byCat.entries()]
          .map(([key, list]) => ({ key, label: key, items: list }))
          .sort((a, b) => b.items.length - a.items.length);
      }
      default: {
        const now = Date.now();
        const today: NewsResponse[] = [];
        const week: NewsResponse[] = [];
        const earlier: NewsResponse[] = [];
        items.forEach((a) => {
          const diff = now - new Date(a.publishDate).getTime();
          if (diff < 24 * 3_600_000) today.push(a);
          else if (diff < 7 * 24 * 3_600_000) week.push(a);
          else earlier.push(a);
        });
        const out: Group[] = [];
        if (today.length) out.push({ key: 'today', label: 'Today', items: today });
        if (week.length) out.push({ key: 'week', label: 'This week', items: week });
        if (earlier.length) out.push({ key: 'earlier', label: 'Earlier', items: earlier });
        return out;
      }
    }
  });

  public async ngOnInit(): Promise<void> {
    await this.load();
  }

  protected async load(): Promise<void> {
    this.isLoading.set(true);
    this.error.set(null);
    try {
      const page = await firstValueFrom(
        this.newsService.getMany({
          take: 100,
          offset: 0,
          orderBy: NewsOrderField.PublishDate,
          orderDirection: OrderDirection.Descending,
        }),
      );
      this.allArticles.set(page.items);
    } catch (e) {
      this.error.set(e instanceof Error ? e.message : 'Failed to load saved articles.');
    } finally {
      this.isLoading.set(false);
    }
  }

  protected setGroupBy(value: GroupBy): void {
    this.groupBy.set(value);
  }

  protected open(article: NewsResponse): void {
    this.navigation.openOriginal(article);
  }

  protected unsave(article: NewsResponse, event: Event): void {
    event.stopPropagation();
    this.savedArticles.toggle(article.id);
    this.toast.info(`Removed "${article.title}" from saved`);
  }

  protected categoryFor = categoryFor;
  protected readTimeFor = readTimeFor;
}
