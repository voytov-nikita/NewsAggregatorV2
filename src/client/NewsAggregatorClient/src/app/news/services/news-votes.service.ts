import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface NewsVoteResponse {
  likes: number;
  dislikes: number;
  myVote: number;
}

@Injectable({ providedIn: 'root' })
export class NewsVotesService {
  private readonly baseUrl: string = environment.newsApiBaseUrl;
  private readonly http = inject(HttpClient);

  /** `value` is +1, -1, or 0 to retract. The server answers with the recounted totals. */
  public vote(newsId: number, value: number): Observable<NewsVoteResponse> {
    return this.http.post<NewsVoteResponse>(`${this.baseUrl}/${newsId}/vote`, { value });
  }

  public get(newsId: number): Observable<NewsVoteResponse> {
    return this.http.get<NewsVoteResponse>(`${this.baseUrl}/${newsId}/vote`);
  }
}
