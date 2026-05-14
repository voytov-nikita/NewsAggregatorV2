import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CommentsResponse,
  CommentsFilterRequest,
  CommentCreateRequest,
  CommentUpdateRequest,
} from '@shared/models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CommentsService {
  private readonly baseUrl: string = environment.newsApiBaseUrl;

  constructor(private http: HttpClient) {}

  public getMany(newsId: number, filter: CommentsFilterRequest): Observable<CommentsResponse[]> {
    const params = new HttpParams()
      .set('take', filter.take)
      .set('offset', filter.offset)
      .set('orderBy', filter.orderBy)
      .set('orderDirection', filter.orderDirection);

    return this.http.get<CommentsResponse[]>(`${this.baseUrl}/${newsId}/comments`, { params });
  }

  public create(newsId: number, request: CommentCreateRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${newsId}/comments`, request);
  }

  public update(newsId: number, commentId: number, request: CommentUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${newsId}/comments/${commentId}`, request);
  }

  public like(newsId: number, commentId: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${newsId}/comments/${commentId}/like`, null);
  }

  public dislike(newsId: number, commentId: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${newsId}/comments/${commentId}/dislike`, null);
  }
}
