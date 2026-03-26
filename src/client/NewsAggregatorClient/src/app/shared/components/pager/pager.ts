import { Component, input, output } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-pager',
  templateUrl: './pager.html',
  styleUrl: './pager.scss',
})
export class Pager {
  readonly currentPage = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly take = input(10);
  readonly pageChange = output<number>();
  readonly takeChange = output<number>();

  protected onPaginatorChange(event: { first?: number; rows?: number }): void {
    const first = event.first ?? 0;
    const rows = event.rows ?? this.take();
    const newPage = Math.floor(first / rows) + 1;
    if (rows !== this.take()) {
      this.takeChange.emit(rows);
    }
    if (newPage !== this.currentPage()) {
      this.pageChange.emit(newPage);
    }
  }
}
