import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Source } from '../models/source.model';

@Injectable({ providedIn: 'root' })
export class SourcesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.crawlerApiBaseUrl}/sources`;

  public getAll(): Observable<Source[]> {
    return this.http.get<Source[]>(this.baseUrl);
  }

  public getById(id: string): Observable<Source> {
    return this.http.get<Source>(`${this.baseUrl}/${encodeURIComponent(id)}`);
  }

  public create(source: Source): Observable<Source> {
    return this.http.post<Source>(this.baseUrl, source);
  }

  public update(source: Source): Observable<Source> {
    return this.http.put<Source>(`${this.baseUrl}/${encodeURIComponent(source.id)}`, source);
  }

  public delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${encodeURIComponent(id)}`);
  }

  public toggle(id: string): Observable<Source> {
    return this.http.put<Source>(`${this.baseUrl}/${encodeURIComponent(id)}/toggle`, {});
  }
}
