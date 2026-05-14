import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

export type VoteValue = 'up' | 'down' | null;

@Component({
  standalone: false,
  selector: 'app-vote-buttons',
  templateUrl: './vote-buttons.html',
  styleUrl: './vote-buttons.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VoteButtons {
  readonly likes = input.required<number>();
  readonly dislikes = input.required<number>();
  readonly value = input<VoteValue>(null);
  readonly compact = input(false);
  readonly vote = output<VoteValue>();

  protected onUp(): void {
    this.vote.emit(this.value() === 'up' ? null : 'up');
  }

  protected onDown(): void {
    this.vote.emit(this.value() === 'down' ? null : 'down');
  }
}
