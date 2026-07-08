import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { NewsCategory, NewsFilterRequest, NewsResponse } from '@shared/models';

export const FeedActions = createActionGroup({
  source: 'Feed',
  events: {
    'Load': props<{ filter: NewsFilterRequest }>(),
    'Load Success': props<{ articles: NewsResponse[]; totalCount: number }>(),
    'Load Failure': props<{ error: string }>(),
    'Set Search': props<{ keyword: string }>(),
    'Set Sort': props<{ sortId: string }>(),
    'Set Categories': props<{ categories: NewsCategory[] }>(),
    'Toggle Category': props<{ category: NewsCategory }>(),
    'Set Sources': props<{ sources: string[] }>(),
    'Toggle Source': props<{ source: string }>(),
    'Set Date Range': props<{ from: string | null; to: string | null }>(),
    'Reset Filters': emptyProps(),
    'Toggle Filter Rail': emptyProps(),
    'Set Page': props<{ page: number }>(),
    'Set Take': props<{ take: number }>(),
    'Clear Error': emptyProps(),
  },
});
