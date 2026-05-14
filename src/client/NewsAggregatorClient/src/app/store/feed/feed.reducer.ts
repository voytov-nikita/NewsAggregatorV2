import { createFeature, createReducer, on } from '@ngrx/store';
import { NewsResponse } from '@shared/models';
import { FeedActions } from './feed.actions';

export interface FeedState {
  articles: NewsResponse[];
  isLoading: boolean;
  error: string | null;
  searchQuery: string;
  selectedSortId: string;
  selectedCategory: string;
  currentPage: number;
  pageSize: number;
}

export const initialFeedState: FeedState = {
  articles: [],
  isLoading: false,
  error: null,
  searchQuery: '',
  selectedSortId: 'newest',
  selectedCategory: 'All',
  currentPage: 1,
  pageSize: 10,
};

export const feedFeature = createFeature({
  name: 'feed',
  reducer: createReducer(
    initialFeedState,
    on(FeedActions.load, (state) => ({ ...state, isLoading: true, error: null })),
    on(FeedActions.loadSuccess, (state, { articles }) => ({
      ...state,
      articles,
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
    on(FeedActions.setCategory, (state, { category }) => ({
      ...state,
      selectedCategory: category,
      currentPage: 1,
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
  selectIsLoading,
  selectError,
  selectSearchQuery,
  selectSelectedSortId,
  selectSelectedCategory,
  selectCurrentPage,
  selectPageSize,
} = feedFeature;
