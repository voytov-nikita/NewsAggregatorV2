import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, MeResponse, RegisterRequest } from '../models/auth.models';

/**
 * Transport only. Every call needs `withCredentials` so the `na_rt` cookie travels - the SPA and the
 * API are different origins, and the browser omits cookies on cross-origin requests otherwise.
 */
@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly baseUrl: string = environment.authApiBaseUrl;
  private readonly http = inject(HttpClient);

  public login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request, { withCredentials: true });
  }

  public register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, request, {
      withCredentials: true,
    });
  }

  public refresh(): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/refresh`, null, { withCredentials: true });
  }

  public logout(allDevices = false): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout?allDevices=${allDevices}`, null, {
      withCredentials: true,
    });
  }

  public me(): Observable<MeResponse> {
    return this.http.get<MeResponse>(`${this.baseUrl}/me`, { withCredentials: true });
  }
}
