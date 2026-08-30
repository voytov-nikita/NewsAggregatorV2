import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { authGuard } from './auth.guard';
import { permissionGuard } from './permission.guard';
import { AuthService } from '../services/auth.service';
import { Permissions } from '../constants/permissions';
import { AuthUser } from '../models/auth.models';

describe('auth guards', () => {
  let auth: AuthService;
  let router: Router;

  const route = {} as ActivatedRouteSnapshot;
  const state = { url: '/admin/sources' } as RouterStateSnapshot;

  // Signing in through the real login call rather than poking at private state, so the test stays
  // honest about how those signals actually get populated.
  const signIn = (permissions: string[]) => {
    const user: AuthUser = {
      id: 'b7f0',
      email: 'a@b.co',
      userName: 'tester',
      displayName: null,
      roles: ['User'],
      permissions,
    };

    auth.login({ email: user.email, password: 'Passw0rd!' }).subscribe();

    TestBed.inject(HttpTestingController)
      .expectOne(`${environment.authApiBaseUrl}/login`)
      .flush({ accessToken: 'token', tokenType: 'Bearer', expiresIn: 900, user });
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });

    auth = TestBed.inject(AuthService);
    router = TestBed.inject(Router);
  });

  afterEach(() => TestBed.inject(HttpTestingController).verify());

  describe('authGuard', () => {
    it('lets a signed-in user through', () => {
      signIn([]);

      expect(TestBed.runInInjectionContext(() => authGuard(route, state))).toBe(true);
    });

    it('redirects an anonymous visitor to the login page, carrying the return url', () => {
      const result = TestBed.runInInjectionContext(() => authGuard(route, state));

      expect(result).toBeInstanceOf(UrlTree);
      expect(router.serializeUrl(result as UrlTree)).toBe(
        '/auth/login?returnUrl=%2Fadmin%2Fsources',
      );
    });
  });

  describe('permissionGuard', () => {
    it('lets a holder of the permission through', () => {
      signIn([Permissions.SourcesManage]);

      const guard = permissionGuard(Permissions.SourcesManage);

      expect(TestBed.runInInjectionContext(() => guard(route, state))).toBe(true);
    });

    it('sends a signed-in user without the permission home, not to the login page', () => {
      // Signing in again would not help them, so bouncing to /auth/login would be a dead end.
      signIn([Permissions.CommentWrite]);

      const guard = permissionGuard(Permissions.SourcesManage);
      const result = TestBed.runInInjectionContext(() => guard(route, state));

      expect(router.serializeUrl(result as UrlTree)).toBe('/allnews');
    });

    it('sends an anonymous visitor to the login page', () => {
      const guard = permissionGuard(Permissions.SourcesManage);
      const result = TestBed.runInInjectionContext(() => guard(route, state));

      expect(router.serializeUrl(result as UrlTree)).toBe('/auth/login?returnUrl=%2Fadmin%2Fsources');
    });
  });
});
