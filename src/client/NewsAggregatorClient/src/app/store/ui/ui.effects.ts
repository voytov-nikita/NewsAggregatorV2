import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { tap, withLatestFrom, map } from 'rxjs';
import { UiActions } from './ui.actions';
import { selectTheme, selectSidebarCollapsed } from './ui.reducer';

const THEME_STORAGE_KEY = 'na-theme';
const SIDEBAR_STORAGE_KEY = 'na-sidebar-collapsed';

@Injectable()
export class UiEffects {
  private readonly actions$ = inject(Actions);
  private readonly store = inject(Store);

  readonly persistTheme$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(UiActions.setTheme, UiActions.toggleTheme),
        withLatestFrom(this.store.select(selectTheme)),
        tap(([, theme]) => {
          try {
            localStorage.setItem(THEME_STORAGE_KEY, theme);
          } catch {
            /* ignore quota / private mode */
          }
          document.documentElement.setAttribute('data-theme', theme);
        }),
      ),
    { dispatch: false },
  );

  readonly persistSidebar$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(UiActions.toggleSidebar, UiActions.setSidebarCollapsed),
        withLatestFrom(this.store.select(selectSidebarCollapsed)),
        tap(([, collapsed]) => {
          try {
            localStorage.setItem(SIDEBAR_STORAGE_KEY, collapsed ? '1' : '0');
          } catch {
            /* ignore */
          }
        }),
      ),
    { dispatch: false },
  );

  readonly applyInitialTheme$ = createEffect(
    () =>
      this.store.select(selectTheme).pipe(
        map((theme) => {
          document.documentElement.setAttribute('data-theme', theme);
        }),
      ),
    { dispatch: false },
  );
}
