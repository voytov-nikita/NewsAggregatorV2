import { Component, computed, input, output } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-pager',
  templateUrl: './pager.html',
  styleUrl: './pager.css',
})
export class Pager {
  readonly currentPage = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly take = input(10);
  readonly pageChange = output<number>();
  readonly takeChange = output<number>();

  protected readonly pages = computed(() =>
    Array.from({ length: this.totalPages() }, (_, i) => i + 1),
  );

  protected setPage(page: number): void {
    if (page >= 1 && page <= this.totalPages() && page !== this.currentPage()) {
      this.pageChange.emit(page);
    }
  }

  protected onTakeInput(event: Event): void {
    const value = +(event.target as HTMLInputElement).value;
    if (value >= 1 && value <= 100 && value !== this.take()) {
      this.takeChange.emit(value);
    }
  }
}
