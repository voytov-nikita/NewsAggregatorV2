import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { environment } from '../../../environments/environment';

/** Endpoints that establish or rotate the session - they must never carry a stale bearer token. */
const SESSION_ENDPOINTS = /\/(login|register|refresh|logout)(\?|$)/;

const API_ORIGINS = [
  environment.authApiBaseUrl,
  environment.newsApiBaseUrl,
  environment.crawlerApiBaseUrl,
];

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  // Attach the token only to our own APIs. A blanket header would leak the session to any third
  // party the app ever calls - an analytics beacon, a CDN, an avatar service.
  if (!API_ORIGINS.some((origin) => request.url.startsWith(origin))) {
    return next(request);
  }

  const auth = inject(AuthService);
  const router = inject(Router);

  const isSessionEndpoint = SESSION_ENDPOINTS.test(request.url);
  const authorized = withBearer(request, isSessionEndpoint ? null : auth.accessToken());

  return next(authorized).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse) || error.status !== 401 || isSessionEndpoint) {
        return throwError(() => error);
      }

      // `next` is the rest of the chain, not this interceptor, so the retry below cannot re-enter
      // here - a second 401 propagates to the caller instead of looping.
      return auth.refresh$().pipe(
        switchMap((token) => next(withBearer(request, token))),
        catchError(() => {
          auth.clear();
          void router.navigate(['/auth/login'], {
            queryParams: { returnUrl: router.url },
          });

          return throwError(() => error);
        }),
      );
    }),
  );
};

function withBearer(request: HttpRequest<unknown>, token: string | null): HttpRequest<unknown> {
  // withCredentials only for AuthService, which enumerates its allowed origins. The other services
  // answer with `Access-Control-Allow-Origin: *`, and a browser rejects that response outright when
  // the request was made with credentials - every news call would fail as a CORS error.
  const prepared = request.url.startsWith(environment.authApiBaseUrl)
    ? request.clone({ withCredentials: true })
    : request;

  return token ? prepared.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : prepared;
}
