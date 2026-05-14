import { Injectable, Signal, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';
import { Theme, UiActions, selectTheme } from '../../store';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly store = inject(Store);

  readonly theme: Signal<Theme> = toSignal(this.store.select(selectTheme), { initialValue: 'dark' });

  toggle(): void {
    this.store.dispatch(UiActions.toggleTheme());
  }

  set(theme: Theme): void {
    this.store.dispatch(UiActions.setTheme({ theme }));
  }
}
