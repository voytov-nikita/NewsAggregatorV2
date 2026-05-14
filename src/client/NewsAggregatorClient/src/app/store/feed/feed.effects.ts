import { inject, Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { NewsService } from '../../news/services/news.service';
import { FeedActions } from './feed.actions';

@Injectable()
export class FeedEffects {
  private readonly actions$ = inject(Actions);
  private readonly newsService = inject(NewsService);

  readonly load$ = createEffect(() =>
    this.actions$.pipe(
      ofType(FeedActions.load),
      switchMap(({ filter }) =>
        this.newsService.getMany(filter).pipe(
          map((articles) => FeedActions.loadSuccess({ articles })),
          catchError((err: HttpErrorResponse) =>
            of(FeedActions.loadFailure({ error: this.formatError(err) })),
          ),
        ),
      ),
    ),
  );

  private formatError(err: HttpErrorResponse): string {
    if (err.status === 0) return 'Cannot reach the news service. Check your connection.';
    if (err.status >= 500) return 'News service is unavailable. Please try again shortly.';
    if (err.status >= 400) return err.error?.message ?? `Request failed (${err.status}).`;
    return err.message ?? 'Unexpected error.';
  }
}
