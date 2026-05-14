import { Component, computed, inject, OnInit } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';
import { NavigationService } from '@shared/services';
import { NewsResponse, NewsFilterRequest } from '@shared/models';
import { NewsOrderField, OrderDirection } from '@shared/enums';
import {
  FeedActions,
  selectArticles,
  selectCurrentPage,
  selectError,
  selectIsLoading,
  selectPageSize,
  selectSearchQuery,
  selectSelectedCategory,
  selectSelectedSortId,
} from '../../../store';

interface SortOption {
  id: string;
  label: string;
  field: NewsOrderField;
  direction: OrderDirection;
}

const SORT_OPTIONS: SortOption[] = [
  { id: 'newest', label: 'Newest', field: NewsOrderField.PublishDate, direction: OrderDirection.Descending },
  { id: 'oldest', label: 'Oldest', field: NewsOrderField.PublishDate, direction: OrderDirection.Ascending },
  { id: 'title-az', label: 'Title A→Z', field: NewsOrderField.Title, direction: OrderDirection.Ascending },
  { id: 'title-za', label: 'Title Z→A', field: NewsOrderField.Title, direction: OrderDirection.Descending },
];

const CATEGORIES = [
  'All',
  'Technology',
  'Architecture',
  'Backend',
  'Frontend',
  'Database',
  'DevOps',
  'Real-time',
  'State',
] as const;

@Component({
  standalone: false,
  selector: 'app-news-list',
  templateUrl: './list.html',
  styleUrl: './list.scss',
})
export class NewsList implements OnInit {
  private readonly store = inject(Store);
  protected readonly navigation = inject(NavigationService);

  protected readonly sortOptions = SORT_OPTIONS;
  protected readonly categories = CATEGORIES;

  protected readonly articles = toSignal(this.store.select(selectArticles), { initialValue: [] });
  protected readonly isLoading = toSignal(this.store.select(selectIsLoading), { initialValue: false });
  protected readonly error = toSignal(this.store.select(selectError), { initialValue: null });
  protected readonly searchQuery = toSignal(this.store.select(selectSearchQuery), { initialValue: '' });
  protected readonly selectedSortId = toSignal(this.store.select(selectSelectedSortId), { initialValue: 'newest' });
  protected readonly selectedCategory = toSignal(this.store.select(selectSelectedCategory), { initialValue: 'All' });
  protected readonly currentPage = toSignal(this.store.select(selectCurrentPage), { initialValue: 1 });
  protected readonly pageSize = toSignal(this.store.select(selectPageSize), { initialValue: 10 });

  protected readonly totalPages = computed(() => {
    const count = this.articles().length;
    return Math.max(1, Math.ceil(count / this.pageSize()) + (count >= this.pageSize() ? 1 : 0));
  });
  protected readonly totalItems = computed<number | null>(() => null);

  protected readonly currentSort = computed(
    () => this.sortOptions.find((s) => s.id === this.selectedSortId()) ?? this.sortOptions[0],
  );

  public ngOnInit(): void {
    this.dispatchLoad();
  }

  protected onSearchChange(value: string): void {
    this.store.dispatch(FeedActions.setSearch({ keyword: value }));
    this.dispatchLoad();
  }

  protected onSortChange(id: string): void {
    this.store.dispatch(FeedActions.setSort({ sortId: id }));
    this.dispatchLoad();
  }

  protected onCategoryChange(category: string): void {
    this.store.dispatch(FeedActions.setCategory({ category }));
    // Server-side category filter not implemented; selection only.
  }

  protected setPage(page: number): void {
    this.store.dispatch(FeedActions.setPage({ page }));
    this.dispatchLoad();
  }

  protected onTakeChange(take: number): void {
    this.store.dispatch(FeedActions.setTake({ take }));
    this.dispatchLoad();
  }

  protected open(article: NewsResponse): void {
    this.navigation.toArticle(article.id);
  }

  protected dismissError(): void {
    this.store.dispatch(FeedActions.clearError());
  }

  protected categoryFor(article: NewsResponse): string {
    const map: Record<string, string> = {
      'Angular Blog': 'Frontend',
      InfoQ: 'Architecture',
      'The New Stack': 'Backend',
      'Postgres Weekly': 'Database',
      'Microsoft Dev Blog': 'Backend',
      'CSS-Tricks': 'Frontend',
      'ngrx team': 'State',
      'MongoDB Blog': 'Database',
    };
    return map[article.publisher] ?? 'Technology';
  }

  protected likesFor(article: NewsResponse): number {
    return ((article.id * 37) % 200) + 12;
  }

  protected dislikesFor(article: NewsResponse): number {
    return (article.id * 7) % 18;
  }

  protected readTimeFor(article: NewsResponse): number {
    const len = article.description?.length ?? 600;
    return Math.max(3, Math.round(len / 200));
  }

  private dispatchLoad(): void {
    this.store.dispatch(FeedActions.load({ filter: this.buildFilter() }));
  }

  private buildFilter(): NewsFilterRequest {
    const sort = this.currentSort();
    return {
      take: this.pageSize(),
      offset: (this.currentPage() - 1) * this.pageSize(),
      keyword: this.searchQuery() || undefined,
      orderBy: sort.field,
      orderDirection: sort.direction,
    };
  }
}
