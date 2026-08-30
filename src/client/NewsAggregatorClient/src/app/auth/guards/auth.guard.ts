import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Returns a UrlTree rather than `false` so the block and the redirect are one navigation - `false`
 * cancels the navigation and leaves the user staring at the page they came from.
 */
export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return (
    auth.isAuthenticated() ||
    router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } })
  );
};
