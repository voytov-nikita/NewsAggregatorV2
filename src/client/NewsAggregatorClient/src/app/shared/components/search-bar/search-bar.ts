import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  effect,
  inject,
  input,
  model,
  output,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-search-bar',
  templateUrl: './search-bar.html',
  styleUrl: './search-bar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchBar {
  readonly value = model<string>('');
  readonly placeholder = input('Search…');
  readonly debounceMs = input(300);
  readonly debounced = output<string>();

  private readonly subject$ = new Subject<string>();
  private readonly destroyRef = inject(DestroyRef);

  constructor() {
    this.subject$
      .pipe(
        debounceTime(this.debounceMs()),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((v) => this.debounced.emit(v));

    effect(() => {
      this.subject$.next(this.value());
    });
  }

  protected onInput(v: string): void {
    this.value.set(v);
  }

  protected clear(): void {
    this.value.set('');
  }
}
