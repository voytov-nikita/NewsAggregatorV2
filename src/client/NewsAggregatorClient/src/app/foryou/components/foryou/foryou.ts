import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { NewsService } from '../../../news/services/news.service';
import { NewsResponse } from '@shared/models';
import { NewsOrderField, OrderDirection } from '@shared/enums';
import { FollowsService, NavigationService, ToastService } from '@shared/services';
import { CATEGORIES, Category, categoryFor, readTimeFor } from '../../../shared/data/news-catalog';

interface ScoredArticle {
  article: NewsResponse;
  score: number;
  reasons: string[];
}

const ONBOARDING_MIN = 3;

@Component({
  standalone: false,
  selector: 'app-foryou',
  templateUrl: './foryou.html',
  styleUrl: './foryou.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ForYou implements OnInit {
  private readonly newsService = inject(NewsService);
  private readonly follows = inject(FollowsService);
  private readonly toast = inject(ToastService);
  protected readonly navigation = inject(NavigationService);

  protected readonly categories = CATEGORIES;
  protected readonly minOnboarding = ONBOARDING_MIN;

  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly articles = signal<NewsResponse[]>([]);

  // Onboarding draft — only used until user saves their interests.
  protected readonly draftCategories = signal<Set<string>>(new Set());

  protected readonly hasFollows = computed(() => {
    const s = this.follows.state();
    return s.categories.size + s.sources.size > 0;
  });

  protected readonly draftCount = computed(() => this.draftCategories().size);
  protected readonly canSaveOnboarding = computed(() => this.draftCount() >= ONBOARDING_MIN);

  protected readonly scored = computed<ScoredArticle[]>(() => {
    const followState = this.follows.state();
    const list = this.articles();
    if (!list.length) return [];

    return list
      .map((article) => {
        const cat = categoryFor(article);
        const reasons: string[] = [];
        let score = 0;
        if (followState.categories.has(cat)) {
          score += 3;
          reasons.push(`category ${cat}`);
        }
        if (followState.sources.has(article.publisher)) {
          score += 4;
          reasons.push(article.publisher);
        }
        return { article, score, reasons };
      })
      .filter((x) => x.score > 0)
      .sort((a, b) => b.score - a.score);
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
          take: 60,
          offset: 0,
          orderBy: NewsOrderField.PublishDate,
          orderDirection: OrderDirection.Descending,
        }),
      );
      this.articles.set(page.items);
    } catch (e) {
      this.error.set(e instanceof Error ? e.message : 'Failed to load articles.');
    } finally {
      this.isLoading.set(false);
    }
  }

  protected toggleDraft(category: Category): void {
    const next = new Set(this.draftCategories());
    if (next.has(category)) next.delete(category);
    else next.add(category);
    this.draftCategories.set(next);
  }

  protected isDrafted(category: Category): boolean {
    return this.draftCategories().has(category);
  }

  protected saveOnboarding(): void {
    if (!this.canSaveOnboarding()) return;
    this.follows.setMany('categories', this.draftCategories());
    this.toast.success(`Following ${this.draftCount()} categories`);
  }

  protected open(article: NewsResponse): void {
    this.navigation.openOriginal(article);
  }

  protected categoryFor = categoryFor;
  protected readTimeFor = readTimeFor;
}
