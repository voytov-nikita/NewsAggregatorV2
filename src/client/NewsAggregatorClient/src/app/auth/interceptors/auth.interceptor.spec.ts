import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router, provideRouter } from '@angular/router';
import { authInterceptor } from './auth.interceptor';
import { AuthService } from '../services/auth.service';
import { TokenStore } from '../services/token-store';
import { environment } from '../../../environments/environment';

describe('authInterceptor', () => {
  let http: HttpClient;
  let controller: HttpTestingController;
  let auth: AuthService;
  let tokens: TokenStore;

  const newsUrl = `${environment.newsApiBaseUrl}/1/comments`;
  const refreshUrl = `${environment.authApiBaseUrl}/refresh`;

  const authResponse = (accessToken: string) => ({
    accessToken,
    tokenType: 'Bearer',
    expiresIn: 900,
    user: {
      id: 'b7f0',
      email: 'a@b.co',
      userName: 'tester',
      displayName: null,
      roles: ['User'],
      permissions: ['comment.write'],
    },
  });

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    http = TestBed.inject(HttpClient);
    controller = TestBed.inject(HttpTestingController);
    auth = TestBed.inject(AuthService);
    tokens = TestBed.inject(TokenStore);
  });

  afterEach(() => controller.verify());

  it('attaches the bearer token to our own APIs', () => {
    tokens.set('token-1');

    http.get(newsUrl).subscribe();

    const request = controller.expectOne(newsUrl);
    expect(request.request.headers.get('Authorization')).toBe('Bearer token-1');
    request.flush({});
  });

  it('never attaches the token to a third-party URL', () => {
    tokens.set('token-1');

    http.get('https://analytics.example.com/collect').subscribe();

    const request = controller.expectOne('https://analytics.example.com/collect');
    expect(request.request.headers.has('Authorization')).toBe(false);
    request.flush({});
  });

  it('sends credentials to AuthService but not to the other services', () => {
    http.post(refreshUrl, null).subscribe({ error: () => undefined });
    expect(controller.expectOne(refreshUrl).request.withCredentials).toBe(true);
    controller.expectNone(refreshUrl);

    http.get(newsUrl).subscribe();
    const news = controller.expectOne(newsUrl);
    expect(news.request.withCredentials).toBe(false);
    news.flush({});
  });

  it('refreshes once for N concurrent 401s and replays every request', () => {
    tokens.set('stale');

    const results: unknown[] = [];
    http.get(`${environment.newsApiBaseUrl}/1`).subscribe((r) => results.push(r));
    http.get(`${environment.newsApiBaseUrl}/2`).subscribe((r) => results.push(r));
    http.get(`${environment.newsApiBaseUrl}/3`).subscribe((r) => results.push(r));

    controller.match((r) => r.url.startsWith(environment.newsApiBaseUrl)).forEach((request) =>
      request.flush(null, { status: 401, statusText: 'Unauthorized' }),
    );

    // The single-flight guard is the whole point: three 401s, one /refresh.
    const refresh = controller.expectOne(refreshUrl);
    refresh.flush(authResponse('fresh-token'));

    const retries = controller.match((r) => r.url.startsWith(environment.newsApiBaseUrl));
    expect(retries.length).toBe(3);
    retries.forEach((request) => {
      expect(request.request.headers.get('Authorization')).toBe('Bearer fresh-token');
      request.flush({ ok: true });
    });

    expect(results.length).toBe(3);
    expect(auth.accessToken()).toBe('fresh-token');
  });

  it('does not try to refresh a failed session endpoint', () => {
    http.post(`${environment.authApiBaseUrl}/login`, {}).subscribe({ error: () => undefined });

    controller
      .expectOne(`${environment.authApiBaseUrl}/login`)
      .flush(null, { status: 401, statusText: 'Unauthorized' });

    controller.expectNone(refreshUrl);
  });

  it('clears the session and redirects when the refresh itself fails', () => {
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    tokens.set('stale');

    let failed = false;
    http.get(newsUrl).subscribe({ error: () => (failed = true) });

    controller.expectOne(newsUrl).flush(null, { status: 401, statusText: 'Unauthorized' });
    controller.expectOne(refreshUrl).flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(failed).toBe(true);
    expect(auth.isAuthenticated()).toBe(false);
    expect(auth.accessToken()).toBeNull();
    expect(navigate).toHaveBeenCalledWith(['/auth/login'], expect.anything());
  });
});
