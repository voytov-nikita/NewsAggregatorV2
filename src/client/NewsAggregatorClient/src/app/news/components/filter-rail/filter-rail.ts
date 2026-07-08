import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';
import { CATEGORIES, CATEGORY_HUES, CATEGORY_LABELS, KNOWN_SOURCES } from '@shared/data/news-catalog';
import { NewsCategory } from '@shared/models';
import {
  FeedActions,
  selectDateFrom,
  selectDateTo,
  selectSelectedCategories,
  selectSelectedSources,
} from '../../../store';

@Component({
  standalone: false,
  selector: 'app-filter-rail',
  templateUrl: './filter-rail.html',
  styleUrl: './filter-rail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FilterRail {
  private readonly store = inject(Store);

  protected readonly categories = CATEGORIES;
  protected readonly hues = CATEGORY_HUES;
  protected readonly labels = CATEGORY_LABELS;
  protected readonly sources = KNOWN_SOURCES;

  protected readonly selectedCategories = toSignal(this.store.select(selectSelectedCategories), { initialValue: [] });
  protected readonly selectedSources = toSignal(this.store.select(selectSelectedSources), { initialValue: [] });
  protected readonly dateFrom = toSignal(this.store.select(selectDateFrom), { initialValue: null });
  protected readonly dateTo = toSignal(this.store.select(selectDateTo), { initialValue: null });

  protected readonly sourceQuery = signal('');

  protected readonly visibleSources = computed(() => {
    const q = this.sourceQuery().trim().toLowerCase();
    if (!q) return this.sources;
    return this.sources.filter((s) => s.toLowerCase().includes(q));
  });

  protected readonly activeCount = computed(() => {
    let n = 0;
    if (this.selectedCategories().length) n++;
    if (this.selectedSources().length) n++;
    if (this.dateFrom() || this.dateTo()) n++;
    return n;
  });

  protected toggleCategory(category: NewsCategory): void {
    this.store.dispatch(FeedActions.toggleCategory({ category }));
  }

  protected isCategorySelected(category: NewsCategory): boolean {
    return this.selectedCategories().includes(category);
  }

  protected toggleSource(source: string): void {
    this.store.dispatch(FeedActions.toggleSource({ source }));
  }

  protected isSourceSelected(source: string): boolean {
    return this.selectedSources().includes(source);
  }

  protected onSourceQuery(value: string): void {
    this.sourceQuery.set(value);
  }

  protected onDateFromChange(value: string): void {
    this.store.dispatch(FeedActions.setDateRange({ from: value || null, to: this.dateTo() }));
  }

  protected onDateToChange(value: string): void {
    this.store.dispatch(FeedActions.setDateRange({ from: this.dateFrom(), to: value || null }));
  }

  protected reset(): void {
    this.store.dispatch(FeedActions.resetFilters());
    this.sourceQuery.set('');
  }
}
