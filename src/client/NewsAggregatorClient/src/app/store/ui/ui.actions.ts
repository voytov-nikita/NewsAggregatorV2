import { createActionGroup, emptyProps, props } from '@ngrx/store';

export const UiActions = createActionGroup({
  source: 'UI',
  events: {
    'Set Theme': props<{ theme: 'dark' | 'light' }>(),
    'Toggle Theme': emptyProps(),
    'Toggle Sidebar': emptyProps(),
    'Set Sidebar Collapsed': props<{ collapsed: boolean }>(),
  },
});
