import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';
import { NavigationService } from '@shared/services';
import { AuthService, Permissions } from '@auth/index';
import { VoteValue } from '@shared/components';
import { NewsVotesService } from '../../services/news-votes.service';
import { NewsResponse, NewsFilterRequest } from '@shared/models';
import { NewsOrderField, OrderDirection } from '@shared/enums';
import { categoryFor, readTimeFor } from '@shared/data/news-catalog';
import {
  FeedActions,
  selectArticles,
  selectCurrentPage,
  selectDateFrom,
  selectDateTo,
  selectError,
  selectFilterRailOpen,
  selectIsLoading,
  selectPageSize,
  selectSearchQuery,
  selectSelectedCategories,
  selectSelectedSortId,
  selectSelectedSources,
  selectTotalCount,
} from '../../../store';

interface SortOption {
  id: string;
  label: string;
  field: NewsOrderField;
  direction: OrderDirection;
}

const SORT_OPTIONS: SortOption[] = [
  { id: 'newest',        label: 'Newest',         field: NewsOrderField.PublishDate,   direction: OrderDirection.Descending },
  { id: 'oldest',        label: 'Oldest',         field: NewsOrderField.PublishDate,   direction: OrderDirection.Ascending },
/*  { id: 'most-liked',    label: 'Most liked',     field: NewsOrderField.MostLiked,     direction: OrderDirection.Descending },
  { id: 'most-discussed',label: 'Most discussed', field: NewsOrderField.MostDiscussed, direction: OrderDirection.Descending },*/
  { id: 'title-az',      label: 'Title A→Z',      field: NewsOrderField.Title,         direction: OrderDirection.Ascending },
  { id: 'title-za',      label: 'Title Z→A',      field: NewsOrderField.Title,         direction: OrderDirection.Descending },
];

@Component({
  standalone: false,
  selector: 'app-news-list',
  templateUrl: './list.html',
  styleUrl: './list.scss',
})
export class NewsList implements OnInit {
  private readonly store = inject(Store);
  protected readonly navigation = inject(NavigationService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly newsVotes = inject(NewsVotesService);

  /** Per-article vote overlay: server-recounted totals for rows this session has voted on. */
  private readonly votes = signal<Record<number, { likes: number; dislikes: number; myVote: number }>>(
    {},
  );

  protected readonly sortOptions = SORT_OPTIONS;

  protected readonly articles = toSignal(this.store.select(selectArticles), { initialValue: [] });
  protected readonly isLoading = toSignal(this.store.select(selectIsLoading), { initialValue: false });
  protected readonly error = toSignal(this.store.select(selectError), { initialValue: null });
  protected readonly searchQuery = toSignal(this.store.select(selectSearchQuery), { initialValue: '' });
  protected readonly selectedSortId = toSignal(this.store.select(selectSelectedSortId), { initialValue: 'newest' });
  protected readonly selectedCategories = toSignal(this.store.select(selectSelectedCategories), { initialValue: [] });
  protected readonly selectedSources = toSignal(this.store.select(selectSelectedSources), { initialValue: [] });
  protected readonly dateFrom = toSignal(this.store.select(selectDateFrom), { initialValue: null });
  protected readonly dateTo = toSignal(this.store.select(selectDateTo), { initialValue: null });
  protected readonly filterRailOpen = toSignal(this.store.select(selectFilterRailOpen), { initialValue: true });
  protected readonly currentPage = toSignal(this.store.select(selectCurrentPage), { initialValue: 1 });
  protected readonly pageSize = toSignal(this.store.select(selectPageSize), { initialValue: 10 });
  protected readonly totalItems = toSignal(this.store.select(selectTotalCount), { initialValue: 0 });

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalItems() / this.pageSize())));

  protected readonly activeFilterCount = computed(() => {
    let n = 0;
    if (this.selectedCategories().length) n++;
    if (this.selectedSources().length) n++;
    if (this.dateFrom() || this.dateTo()) n++;
    return n;
  });

  protected readonly hasActiveFilters = computed(
    () => !!this.searchQuery() || this.activeFilterCount() > 0,
  );

  public ngOnInit(): void {
    // Initial load — subsequent reloads are driven by FeedEffects.autoReload$.
    this.store.dispatch(FeedActions.load({ filter: this.buildFilter() }));
  }

  protected onSearchChange(value: string): void {
    this.store.dispatch(FeedActions.setSearch({ keyword: value }));
  }

  protected onSortChange(id: string): void {
    this.store.dispatch(FeedActions.setSort({ sortId: id }));
  }

  protected setPage(page: number): void {
    this.store.dispatch(FeedActions.setPage({ page }));
  }

  protected onTakeChange(take: number): void {
    this.store.dispatch(FeedActions.setTake({ take }));
  }

  protected toggleFilterRail(): void {
    this.store.dispatch(FeedActions.toggleFilterRail());
  }

  protected resetFilters(): void {
    this.store.dispatch(FeedActions.resetFilters());
  }

  protected dismissError(): void {
    this.store.dispatch(FeedActions.clearError());
  }

  protected reload(): void {
    this.store.dispatch(FeedActions.clearError());
    this.store.dispatch(FeedActions.load({ filter: this.buildFilter() }));
  }

  protected categoryFor = (article: NewsResponse): string => categoryFor(article);
  protected readTimeFor = (article: NewsResponse): number => readTimeFor(article);

  protected likesFor(article: NewsResponse): number {
    return this.votes()[article.id]?.likes ?? article.likes ?? 0;
  }

  protected dislikesFor(article: NewsResponse): number {
    return this.votes()[article.id]?.dislikes ?? article.dislikes ?? 0;
  }

  protected voteFor(article: NewsResponse): VoteValue {
    const value = this.votes()[article.id]?.myVote ?? 0;

    return value > 0 ? 'up' : value < 0 ? 'down' : null;
  }

  /**
   * Anonymous visitors are sent to the login page instead of firing a request the API would reject
   * with a 401 - and a visitor without `news.vote` gets the same treatment, since re-authenticating
   * is not what they need either.
   */
  protected onVote(article: NewsResponse, value: VoteValue): void {
    if (!this.auth.isAuthenticated()) {
      void this.router.navigate(['/auth/login'], {
        queryParams: { returnUrl: this.router.url },
      });

      return;
    }

    if (!this.auth.has(Permissions.NewsVote)) {
      return;
    }

    const numeric = value === 'up' ? 1 : value === 'down' ? -1 : 0;

    // The feed lives in NgRx, but a vote only changes two counters on one row. Keeping the result in
    // a local overlay avoids a reducer whose only job is to patch a number, and the next load reads
    // the recounted values from the server anyway.
    this.newsVotes.vote(article.id, numeric).subscribe((result) =>
      this.votes.update((current) => ({ ...current, [article.id]: result })),
    );
  }

  private buildFilter(): NewsFilterRequest {
    const sort = this.sortOptions.find((s) => s.id === this.selectedSortId()) ?? this.sortOptions[0];
    return {
      take: this.pageSize(),
      offset: (this.currentPage() - 1) * this.pageSize(),
      keyword: this.searchQuery() || undefined,
      orderBy: sort.field,
      orderDirection: sort.direction,
      categories: this.selectedCategories().length ? this.selectedCategories() : undefined,
      sources: this.selectedSources().length ? this.selectedSources() : undefined,
      dateFrom: this.dateFrom() ?? undefined,
      dateTo: this.dateTo() ?? undefined,
    };
  }
}
