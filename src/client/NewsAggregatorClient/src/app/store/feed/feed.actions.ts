import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { NewsFilterRequest, NewsResponse } from '@shared/models';

export const FeedActions = createActionGroup({
  source: 'Feed',
  events: {
    'Load': props<{ filter: NewsFilterRequest }>(),
    'Load Success': props<{ articles: NewsResponse[] }>(),
    'Load Failure': props<{ error: string }>(),
    'Set Search': props<{ keyword: string }>(),
    'Set Sort': props<{ sortId: string }>(),
    'Set Category': props<{ category: string }>(),
    'Set Page': props<{ page: number }>(),
    'Set Take': props<{ take: number }>(),
    'Clear Error': emptyProps(),
  },
});
