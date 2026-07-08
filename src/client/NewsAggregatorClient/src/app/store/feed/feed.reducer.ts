import { createFeature, createReducer, on } from '@ngrx/store';
import { NewsCategory, NewsResponse } from '@shared/models';
import { FeedActions } from './feed.actions';

export interface FeedState {
  articles: NewsResponse[];
  totalCount: number;
  isLoading: boolean;
  error: string | null;
  searchQuery: string;
  selectedSortId: string;
  selectedCategories: NewsCategory[];
  selectedSources: string[];
  dateFrom: string | null;
  dateTo: string | null;
  filterRailOpen: boolean;
  currentPage: number;
  pageSize: number;
}

export const initialFeedState: FeedState = {
  articles: [],
  totalCount: 0,
  isLoading: false,
  error: null,
  searchQuery: '',
  selectedSortId: 'newest',
  selectedCategories: [],
  selectedSources: [],
  dateFrom: null,
  dateTo: null,
  filterRailOpen: true,
  currentPage: 1,
  pageSize: 10,
};

function toggleInList<T>(list: T[], value: T): T[] {
  return list.includes(value) ? list.filter((x) => x !== value) : [...list, value];
}

export const feedFeature = createFeature({
  name: 'feed',
  reducer: createReducer(
    initialFeedState,
    on(FeedActions.load, (state) => ({ ...state, isLoading: true, error: null })),
    on(FeedActions.loadSuccess, (state, { articles, totalCount }) => ({
      ...state,
      articles,
      totalCount,
      isLoading: false,
      error: null,
    })),
    on(FeedActions.loadFailure, (state, { error }) => ({
      ...state,
      isLoading: false,
      error,
    })),
    on(FeedActions.setSearch, (state, { keyword }) => ({
      ...state,
      searchQuery: keyword,
      currentPage: 1,
    })),
    on(FeedActions.setSort, (state, { sortId }) => ({
      ...state,
      selectedSortId: sortId,
      currentPage: 1,
    })),
    on(FeedActions.setCategories, (state, { categories }) => ({
      ...state,
      selectedCategories: categories,
      currentPage: 1,
    })),
    on(FeedActions.toggleCategory, (state, { category }) => ({
      ...state,
      selectedCategories: toggleInList(state.selectedCategories, category),
      currentPage: 1,
    })),
    on(FeedActions.setSources, (state, { sources }) => ({
      ...state,
      selectedSources: sources,
      currentPage: 1,
    })),
    on(FeedActions.toggleSource, (state, { source }) => ({
      ...state,
      selectedSources: toggleInList(state.selectedSources, source),
      currentPage: 1,
    })),
    on(FeedActions.setDateRange, (state, { from, to }) => ({
      ...state,
      dateFrom: from,
      dateTo: to,
      currentPage: 1,
    })),
    on(FeedActions.resetFilters, (state) => ({
      ...state,
      searchQuery: '',
      selectedCategories: [],
      selectedSources: [],
      dateFrom: null,
      dateTo: null,
      currentPage: 1,
    })),
    on(FeedActions.toggleFilterRail, (state) => ({
      ...state,
      filterRailOpen: !state.filterRailOpen,
    })),
    on(FeedActions.setPage, (state, { page }) => ({ ...state, currentPage: page })),
    on(FeedActions.setTake, (state, { take }) => ({
      ...state,
      pageSize: take,
      currentPage: 1,
    })),
    on(FeedActions.clearError, (state) => ({ ...state, error: null })),
  ),
});

export const {
  name: feedFeatureKey,
  reducer: feedReducer,
  selectArticles,
  selectTotalCount,
  selectIsLoading,
  selectError,
  selectSearchQuery,
  selectSelectedSortId,
  selectSelectedCategories,
  selectSelectedSources,
  selectDateFrom,
  selectDateTo,
  selectFilterRailOpen,
  selectCurrentPage,
  selectPageSize,
} = feedFeature;
