import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NewsResponse, NewsFilterRequest } from '@shared/models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class NewsService {
  private readonly baseUrl: string = environment.newsApiBaseUrl;

  constructor(private http: HttpClient) {}

  public getMany(filter: NewsFilterRequest): Observable<NewsResponse[]> {
    let params = new HttpParams()
      .set('Take', filter.take)
      .set('Offset', filter.offset);

    if (filter.keyword) {
      params = params.set('keyword', filter.keyword);
    }

    return this.http.get<NewsResponse[]>(this.baseUrl, { params });
  }
}
