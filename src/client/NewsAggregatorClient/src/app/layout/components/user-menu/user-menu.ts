import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@auth/index';

@Component({
  standalone: false,
  selector: 'app-user-menu',
  templateUrl: './user-menu.html',
  styleUrl: './user-menu.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserMenu {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly open = signal(false);
  protected readonly user = this.auth.user;
  protected readonly isAuthenticated = this.auth.isAuthenticated;

  protected readonly displayName = computed(() => {
    const user = this.user();

    return user ? (user.displayName ?? user.userName) : '';
  });

  /** One badge, the most privileged role the account holds. */
  protected readonly role = computed(() => {
    const roles = this.auth.roles();

    return roles.includes('Admin') ? 'Admin' : (roles[0] ?? null);
  });

  protected toggle(): void {
    this.open.update((value) => !value);
  }

  protected close(): void {
    this.open.set(false);
  }

  protected signIn(): void {
    // Carry the current page so signing in returns the visitor to where they were.
    void this.router.navigate(['/auth/login'], { queryParams: { returnUrl: this.router.url } });
  }

  protected signOut(): void {
    this.close();

    // Navigating home matters: the visitor may be standing on a guarded page, and staying there
    // after losing the session would just bounce them to the login screen.
    this.auth.logout().subscribe(() => void this.router.navigateByUrl('/allnews'));
  }
}
