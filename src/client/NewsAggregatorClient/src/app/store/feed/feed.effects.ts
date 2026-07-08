import { inject, Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { catchError, debounceTime, map, of, switchMap, withLatestFrom } from 'rxjs';
import { NewsService } from '../../news/services/news.service';
import { NewsFilterRequest } from '@shared/models';
import { NewsOrderField, OrderDirection } from '@shared/enums';
import { FeedActions } from './feed.actions';
import { feedFeature, FeedState } from './feed.reducer';

interface SortOption {
  id: string;
  field: NewsOrderField;
  direction: OrderDirection;
}

const SORTS: Record<string, SortOption> = {
  'newest':         { id: 'newest',         field: NewsOrderField.PublishDate,   direction: OrderDirection.Descending },
  'oldest':         { id: 'oldest',         field: NewsOrderField.PublishDate,   direction: OrderDirection.Ascending },
/*  'most-liked':     { id: 'most-liked',     field: NewsOrderField.MostLiked,     direction: OrderDirection.Descending },
  'most-discussed': { id: 'most-discussed', field: NewsOrderField.MostDiscussed, direction: OrderDirection.Descending },*/
  'title-az':       { id: 'title-az',       field: NewsOrderField.Title,         direction: OrderDirection.Ascending },
  'title-za':       { id: 'title-za',       field: NewsOrderField.Title,         direction: OrderDirection.Descending },
};

function buildFilter(state: FeedState): NewsFilterRequest {
  const sort = SORTS[state.selectedSortId] ?? SORTS['newest'];
  return {
    take: state.pageSize,
    offset: (state.currentPage - 1) * state.pageSize,
    keyword: state.searchQuery || undefined,
    orderBy: sort.field,
    orderDirection: sort.direction,
    categories: state.selectedCategories.length ? state.selectedCategories : undefined,
    sources: state.selectedSources.length ? state.selectedSources : undefined,
    dateFrom: state.dateFrom ?? undefined,
    dateTo: state.dateTo ?? undefined,
  };
}

@Injectable()
export class FeedEffects {
  private readonly actions$ = inject(Actions);
  private readonly store = inject(Store);
  private readonly newsService = inject(NewsService);

  readonly load$ = createEffect(() =>
    this.actions$.pipe(
      ofType(FeedActions.load),
      switchMap(({ filter }) =>
        this.newsService.getMany(filter).pipe(
          map((page) =>
            FeedActions.loadSuccess({ articles: page.items, totalCount: page.totalCount }),
          ),
          catchError((err: HttpErrorResponse) =>
            of(FeedActions.loadFailure({ error: this.formatError(err) })),
          ),
        ),
      ),
    ),
  );

  /**
   * Re-fetches the feed whenever any filter / sort / pager action fires.
   * Debounced so slider drags and rapid checkbox toggles coalesce into one request.
   */
  readonly autoReload$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        FeedActions.setSearch,
        FeedActions.setSort,
        FeedActions.toggleCategory,
        FeedActions.setCategories,
        FeedActions.toggleSource,
        FeedActions.setSources,
        FeedActions.setDateRange,
        FeedActions.resetFilters,
        FeedActions.setPage,
        FeedActions.setTake,
      ),
      debounceTime(200),
      withLatestFrom(this.store.select(feedFeature.selectFeedState)),
      map(([, state]) => FeedActions.load({ filter: buildFilter(state) })),
    ),
  );

  private formatError(err: HttpErrorResponse): string {
    if (err.status === 0) return 'Cannot reach the news service. Check your connection.';
    if (err.status >= 500) return 'News service is unavailable. Please try again shortly.';
    if (err.status >= 400) return err.error?.message ?? `Request failed (${err.status}).`;
    return err.message ?? 'Unexpected error.';
  }
}
