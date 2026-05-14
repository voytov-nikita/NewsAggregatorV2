import { ChangeDetectionStrategy, Component, input, model } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-category-chips',
  templateUrl: './category-chips.html',
  styleUrl: './category-chips.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoryChips {
  readonly categories = input.required<readonly string[]>();
  readonly selected = model<string>('All');

  protected select(category: string): void {
    this.selected.set(category);
  }
}
