import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

const ELLIPSIS = '…';

@Component({
  standalone: false,
  selector: 'app-pager',
  templateUrl: './pager.html',
  styleUrl: './pager.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Pager {
  readonly currentPage = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly take = input(10);
  readonly takeOptions = input<readonly number[]>([5, 10, 25, 50]);
  readonly totalItems = input<number | null>(null);

  readonly pageChange = output<number>();
  readonly takeChange = output<number>();

  protected readonly pages = computed<(number | typeof ELLIPSIS)[]>(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);

    const items: (number | typeof ELLIPSIS)[] = [1];
    const start = Math.max(2, current - 1);
    const end = Math.min(total - 1, current + 1);

    if (start > 2) items.push(ELLIPSIS);
    for (let i = start; i <= end; i++) items.push(i);
    if (end < total - 1) items.push(ELLIPSIS);

    items.push(total);
    return items;
  });

  protected readonly rangeLabel = computed(() => {
    const total = this.totalItems();
    if (total === null) return null;
    const rows = this.take();
    const from = (this.currentPage() - 1) * rows + 1;
    const to = Math.min(total, this.currentPage() * rows);
    return `${from}–${to} of ${total}`;
  });

  protected readonly isEllipsis = (item: number | typeof ELLIPSIS): item is typeof ELLIPSIS =>
    item === ELLIPSIS;

  protected goto(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.currentPage()) return;
    this.pageChange.emit(page);
  }

  protected prev(): void {
    this.goto(this.currentPage() - 1);
  }

  protected next(): void {
    this.goto(this.currentPage() + 1);
  }

  protected onTakeChange(value: string | number): void {
    const next = Number(value);
    if (!Number.isFinite(next) || next === this.take()) return;
    this.takeChange.emit(next);
  }
}
