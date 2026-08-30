import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * A signed-out visitor is sent to the login page; a signed-in one who simply lacks the permission is
 * sent home, because logging in again would not help them.
 *
 * This only hides a screen. The API enforces the same permission on every call, so a hand-typed URL
 * buys nothing but an empty page.
 */
export const permissionGuard =
  (permission: string): CanActivateFn =>
  (_route, state) => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!auth.isAuthenticated()) {
      return router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } });
    }

    return auth.has(permission) || router.createUrlTree(['/allnews']);
  };
