import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { NewsResponse, NewsFilterRequest } from '@shared/models';
import { environment } from '../../../environments/environment';

export interface NewsPage {
  items: NewsResponse[];
  totalCount: number;
  offset: number;
  returned: number;
}

@Injectable({
  providedIn: 'root',
})
export class NewsService {
  private readonly baseUrl: string = environment.newsApiBaseUrl;

  constructor(private http: HttpClient) {}

  public getMany(filter: NewsFilterRequest): Observable<NewsPage> {
    let params = new HttpParams()
      .set('Take', filter.take)
      .set('Offset', filter.offset);

    if (filter.orderBy !== undefined) {
      params = params.set('OrderBy', filter.orderBy.toString());
    }
    if (filter.orderDirection !== undefined) {
      params = params.set('OrderDirection', filter.orderDirection.toString());
    }
    if (filter.keyword) {
      params = params.set('keyword', filter.keyword);
    }
    if (filter.categories?.length) {
      filter.categories.forEach((c) => (params = params.append('categories', c)));
    } else if (filter.category) {
      // Single-category compatibility (chips UI). The server accepts `categories[]`.
      params = params.append('categories', filter.category);
    }
    if (filter.sources?.length) {
      filter.sources.forEach((s) => (params = params.append('sources', s)));
    }
    if (filter.dateFrom) params = params.set('dateFrom', filter.dateFrom);
    if (filter.dateTo) params = params.set('dateTo', filter.dateTo);

    return this.http
      .get<NewsResponse[]>(this.baseUrl, { params, observe: 'response' })
      .pipe(
        map((res) => {
          const items = res.body ?? [];
          const total = Number(res.headers.get('X-Pagination-Total') ?? items.length);
          const offset = Number(res.headers.get('X-Pagination-Offset') ?? filter.offset);
          const returned = Number(res.headers.get('X-Pagination-Returned') ?? items.length);
          return { items, totalCount: total, offset, returned };
        }),
      );
  }
}
