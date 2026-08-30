import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, finalize, firstValueFrom, map, of, shareReplay, tap } from 'rxjs';
import { AuthApiService } from './auth-api.service';
import { TokenStore } from './token-store';
import { AuthResponse, AuthUser, LoginRequest, RegisterRequest } from '../models/auth.models';

/**
 * Signal-based auth state rather than a fourth NgRx feature.
 *
 * Guards and the HTTP interceptor need the current user synchronously and outside the component
 * tree; dispatching an action and then waiting for an effect is exactly what NgRx makes awkward on
 * purpose. The state is also small and single-writer, and `FollowsService` already sets this
 * precedent in the codebase.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(AuthApiService);
  private readonly tokens = inject(TokenStore);

  private readonly _user = signal<AuthUser | null>(null);

  /** The single in-flight refresh, so N simultaneous 401s produce one network call. */
  private refreshInFlight: Observable<string> | null = null;

  public readonly user = this._user.asReadonly();
  public readonly isAuthenticated = computed(() => this._user() !== null);
  public readonly permissions = computed<ReadonlySet<string>>(
    () => new Set(this._user()?.permissions ?? []),
  );
  public readonly roles = computed<readonly string[]>(() => this._user()?.roles ?? []);
  public readonly accessToken = this.tokens.accessToken;

  public has(permission: string): boolean {
    return this.permissions().has(permission);
  }

  public login(request: LoginRequest): Observable<AuthUser> {
    return this.api.login(request).pipe(
      tap((response) => this.apply(response)),
      map((response) => response.user),
    );
  }

  public register(request: RegisterRequest): Observable<AuthUser> {
    return this.api.register(request).pipe(
      tap((response) => this.apply(response)),
      map((response) => response.user),
    );
  }

  public logout(allDevices = false): Observable<void> {
    return this.api.logout(allDevices).pipe(
      catchError(() => of(void 0)),
      finalize(() => this.clear()),
    );
  }

  /**
   * Rotates the session and returns the fresh access token. Concurrent callers share one request:
   * the observable is cached until it settles, so ten queued 401s trigger a single /refresh and are
   * then all replayed against the new token.
   */
  public refresh$(): Observable<string> {
    if (this.refreshInFlight) {
      return this.refreshInFlight;
    }

    this.refreshInFlight = this.api.refresh().pipe(
      tap((response) => this.apply(response)),
      map((response) => response.accessToken),
      finalize(() => (this.refreshInFlight = null)),
      shareReplay({ bufferSize: 1, refCount: false }),
    );

    return this.refreshInFlight;
  }

  /**
   * Called once at bootstrap. A valid `na_rt` cookie silently restores the session across a reload;
   * anything else just leaves the visitor anonymous, which is a supported state, not an error.
   */
  public restore(): Promise<void> {
    return firstValueFrom(this.refresh$().pipe(catchError(() => of('')))).then(() => undefined);
  }

  public clear(): void {
    this._user.set(null);
    this.tokens.clear();
  }

  private apply(response: AuthResponse): void {
    this.tokens.set(response.accessToken);
    this._user.set(response.user);
  }
}
