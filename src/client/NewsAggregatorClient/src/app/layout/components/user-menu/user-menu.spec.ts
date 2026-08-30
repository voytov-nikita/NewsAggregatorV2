import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from '@auth/index';
import { environment } from '../../../../environments/environment';
import { UserMenu } from './user-menu';
// Imported for compilation scope: LayoutModule is what declares UserMenu and brings in app-avatar.
import LayoutModule from '../../layout.module';

describe('UserMenu', () => {
  let controller: HttpTestingController;
  let auth: AuthService;

  const signIn = (roles: string[]) => {
    auth.login({ email: 'a@b.co', password: 'Passw0rd!' }).subscribe();
    controller.expectOne(`${environment.authApiBaseUrl}/login`).flush({
      accessToken: 'token',
      tokenType: 'Bearer',
      expiresIn: 900,
      user: {
        id: 'b7f0',
        email: 'a@b.co',
        userName: 'tester',
        displayName: 'Tester',
        roles,
        permissions: [],
      },
    });
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [LayoutModule],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });

    controller = TestBed.inject(HttpTestingController);
    auth = TestBed.inject(AuthService);
  });

  afterEach(() => controller.verify());

  const render = () => {
    const fixture = TestBed.createComponent(UserMenu);
    fixture.detectChanges();

    return fixture.nativeElement as HTMLElement;
  };

  it('offers Sign in to an anonymous visitor', () => {
    const element = render();

    expect(element.querySelector('.signin-trigger')?.textContent?.trim()).toBe('Sign in');
    expect(element.querySelector('.user-trigger')).toBeNull();
  });

  it('shows the account trigger once signed in', () => {
    signIn(['User']);

    const element = render();

    expect(element.querySelector('.user-trigger')).toBeTruthy();
    expect(element.querySelector('.signin-trigger')).toBeNull();
  });

  it('reveals the identity and a sign-out action when opened', () => {
    signIn(['User']);

    const fixture = TestBed.createComponent(UserMenu);
    fixture.detectChanges();
    (fixture.nativeElement as HTMLElement).querySelector<HTMLButtonElement>('.user-trigger')!.click();
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('.dropdown-identity__name')?.textContent?.trim()).toBe('Tester');
    expect(element.querySelector('.dropdown-identity__email')?.textContent?.trim()).toBe('a@b.co');
    expect(element.querySelector('.dropdown-item')?.textContent?.trim()).toBe('Sign out');
  });

  it('badges the most privileged role the account holds', () => {
    signIn(['User', 'Admin']);

    const fixture = TestBed.createComponent(UserMenu);
    fixture.detectChanges();
    (fixture.nativeElement as HTMLElement).querySelector<HTMLButtonElement>('.user-trigger')!.click();
    fixture.detectChanges();

    expect(
      (fixture.nativeElement as HTMLElement).querySelector('.role-badge')?.textContent?.trim(),
    ).toBe('Admin');
  });
});
