import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { NavigationService } from '@shared/services';

interface DiscussionComment {
  id: number;
  author: string;
  time: string;
  content: string;
  likes: number;
  dislikes: number;
}

@Component({
  standalone: false,
  selector: 'app-news-detail',
  templateUrl: './detail.html',
  styleUrl: './detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NewsDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly navigation = inject(NavigationService);

  protected readonly articleId = toSignal(
    this.route.paramMap.pipe(map((p) => Number(p.get('id')))),
    { initialValue: 0 },
  );

  protected readonly article = computed(() => ({
    id: this.articleId(),
    title:
      'Angular 21 Signals: Complete Migration Guide for Large Codebases',
    publisher: 'Angular Blog',
    category: 'Frontend',
    publishDate: new Date(Date.now() - 3 * 3_600_000).toISOString(),
    readTime: 6,
    likes: 142,
    dislikes: 8,
    paragraphs: [
      'A comprehensive guide to migrating existing Angular applications to use the new Signals API, covering common patterns and performance considerations for enterprise scale.',
      'Signals provide a fine-grained reactivity primitive. Unlike Observables, computed signals lazily recompute only when read after a dependency change — a cheaper baseline for component state.',
      'The interop helpers `toSignal` and `toObservable` make incremental migration feasible. Existing RxJS pipelines stay; new code adopts signals where reactive read paths are dominant.',
    ],
    sourceUrl: '#',
  }));

  protected readonly comments = signal<DiscussionComment[]>([
    {
      id: 1,
      author: 'alex_dev',
      time: '2h ago',
      content:
        'Great article! The migration path from NgModules to standalone components combined with Signals is exactly what I needed.',
      likes: 24,
      dislikes: 1,
    },
    {
      id: 2,
      author: 'typescript_fan',
      time: '1h 45m ago',
      content:
        'Worth noting: computed signals have lazy evaluation — they only recompute when dependent signals change AND the computed value is accessed.',
      likes: 18,
      dislikes: 0,
    },
    {
      id: 3,
      author: 'nina_k',
      time: '1h 20m ago',
      content:
        'Been using this pattern in production for 3 months. Debugging experience is much better than manually tracking Observable subscriptions.',
      likes: 12,
      dislikes: 2,
    },
  ]);

  protected readonly commentDraft = signal('');

  protected updateDraft(value: string): void {
    this.commentDraft.set(value);
  }

  protected submitComment(): void {
    const content = this.commentDraft().trim();
    if (!content) return;
    this.comments.update((list) => [
      ...list,
      {
        id: Date.now(),
        author: 'you',
        time: 'just now',
        content,
        likes: 0,
        dislikes: 0,
      },
    ]);
    this.commentDraft.set('');
  }

  protected back(): void {
    this.navigation.toFeed();
  }
}
