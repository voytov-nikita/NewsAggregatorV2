import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { TokenStore } from './token-store';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let auth: AuthService;
  let controller: HttpTestingController;

  const base = environment.authApiBaseUrl;

  const authResponse = (permissions: string[] = ['comment.write']) => ({
    accessToken: 'token',
    tokenType: 'Bearer',
    expiresIn: 900,
    user: {
      id: 'b7f0',
      email: 'a@b.co',
      userName: 'tester',
      displayName: 'Tester',
      roles: ['User'],
      permissions,
    },
  });

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    auth = TestBed.inject(AuthService);
    controller = TestBed.inject(HttpTestingController);
  });

  afterEach(() => controller.verify());

  it('starts anonymous', () => {
    expect(auth.isAuthenticated()).toBe(false);
    expect(auth.accessToken()).toBeNull();
    expect(auth.has('comment.write')).toBe(false);
  });

  it('publishes the user and permissions after a login', () => {
    auth.login({ email: 'a@b.co', password: 'Passw0rd!' }).subscribe();
    controller.expectOne(`${base}/login`).flush(authResponse());

    expect(auth.isAuthenticated()).toBe(true);
    expect(auth.accessToken()).toBe('token');
    expect(auth.has('comment.write')).toBe(true);
    expect(auth.has('sources.manage')).toBe(false);
  });

  it('sends credentials on login so the refresh cookie is set', () => {
    auth.login({ email: 'a@b.co', password: 'Passw0rd!' }).subscribe();

    const request = controller.expectOne(`${base}/login`);
    expect(request.request.withCredentials).toBe(true);
    request.flush(authResponse());
  });

  it('restores a session from the refresh cookie at startup', async () => {
    const restored = auth.restore();
    controller.expectOne(`${base}/refresh`).flush(authResponse());
    await restored;

    expect(auth.isAuthenticated()).toBe(true);
  });

  it('leaves the visitor anonymous when there is no valid cookie, without throwing', async () => {
    const restored = auth.restore();
    controller
      .expectOne(`${base}/refresh`)
      .flush(null, { status: 401, statusText: 'Unauthorized' });

    await expect(restored).resolves.toBeUndefined();
    expect(auth.isAuthenticated()).toBe(false);
  });

  it('clears the session even when the logout call itself fails', () => {
    auth.login({ email: 'a@b.co', password: 'Passw0rd!' }).subscribe();
    controller.expectOne(`${base}/login`).flush(authResponse());

    auth.logout().subscribe();
    controller
      .expectOne(`${base}/logout?allDevices=false`)
      .flush(null, { status: 500, statusText: 'Server Error' });

    // A failed server call must not strand the browser in a half-signed-in state.
    expect(auth.isAuthenticated()).toBe(false);
    expect(auth.accessToken()).toBeNull();
  });

  it('starts a new refresh once the previous one has settled', () => {
    TestBed.inject(TokenStore).set('stale');

    auth.refresh$().subscribe();
    controller.expectOne(`${base}/refresh`).flush(authResponse());

    auth.refresh$().subscribe();
    controller.expectOne(`${base}/refresh`).flush(authResponse());

    expect(auth.isAuthenticated()).toBe(true);
  });
});
