import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, finalize, Subject, switchMap, tap } from 'rxjs';
import { NavigationService } from '@shared/services';
import { NewsResponse, NewsFilterRequest } from '@shared/models';
import { NewsOrderField, OrderDirection } from '@shared/enums';
import { NewsService } from '../../services/news.service';

@Component({
  standalone: false,
  selector: 'app-news-list',
  templateUrl: './list.html',
  styleUrl: './list.css',
})
export class NewsList implements OnInit {
  private destroyRef = inject(DestroyRef);
  private newsService = inject(NewsService);

  protected readonly pageSize = signal(10);

  protected readonly isLoading = signal(false);
  protected readonly currentPage = signal(1);
  protected readonly totalPages = signal(5);
  protected readonly searchQuery = signal('');
  protected readonly articles = signal<NewsResponse[]>([]);

  private _onSearch$ = new Subject<string>();

  constructor(protected readonly navigation: NavigationService) {
    this._onSearch$
      .pipe(
        debounceTime(300),
        tap(_ => {
          this.currentPage.set(1);
          this.isLoading.set(true);
        }),
        switchMap(keyword => {
          const filter = this.buildFilter(keyword || undefined);
          return this.newsService.getMany(filter).pipe(
            finalize(() => this.isLoading.set(false)),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(articles => this.articles.set(articles));
  }

  public ngOnInit(): void {
    this.loadArticles();
  }

  protected onSearchInput(value: string): void {
    this.searchQuery.set(value);
    this._onSearch$.next(value);
  }

  protected setPage(page: number): void {
    if (page >= 1 && page <= this.totalPages() && page !== this.currentPage()) {
      this.currentPage.set(page);
      this.loadArticles();
    }
  }

  protected onTakeChange(take: number): void {
    this.pageSize.set(take);
    this.currentPage.set(1);
    this.loadArticles();
  }

  private loadArticles(): void {
    const keyword = this.searchQuery() || undefined;
    const filter = this.buildFilter(keyword);

    this.isLoading.set(true);
    this.newsService
      .getMany(filter)
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(articles => this.articles.set(articles));
  }

  private buildFilter(keyword?: string): NewsFilterRequest {
    return {
      take: this.pageSize(),
      offset: (this.currentPage() - 1) * this.pageSize(),
      keyword,
      orderBy: NewsOrderField.PublishDate,
      orderDirection: OrderDirection.Descending,
    };
  }
}
