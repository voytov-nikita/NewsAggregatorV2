import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map, startWith } from 'rxjs';
import { NavItem } from '@shared/components';
import { NavigationService } from '@shared/services';
import { AuthService, Permissions } from '@auth/index';

@Component({
  standalone: false,
  selector: 'app-authorized-layout',
  templateUrl: './authorized-layout.html',
  styleUrl: './authorized-layout.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthorizedLayout {
  private readonly router = inject(Router);
  protected readonly navigation = inject(NavigationService);

  private readonly auth = inject(AuthService);

  /**
   * Computed so the Admin entry appears and disappears with the signed-in user's permissions. Each
   * child is checked separately - an account may hold one of the two admin permissions and not the
   * other, and a link to a screen its guard will bounce is worse than no link.
   */
  protected readonly navItems = computed<NavItem[]>(() => {
    const items: NavItem[] = [
      { id: 'foryou', label: 'For you', icon: '★', route: '/foryou' },
      { id: 'allnews', label: 'All news', icon: '⊞', route: '/allnews' },
      { id: 'trending', label: 'Trending', icon: '', iconKind: 'flame', route: '/trending' },
      { id: 'subscriptions', label: 'Subscriptions', icon: '☆', route: '/subscriptions' },
      { id: 'saved', label: 'Saved', icon: '⊕', route: '/saved' },
    ];

    const adminChildren = [
      ...(this.auth.has(Permissions.SourcesManage)
        ? [{ id: 'admin-sources', label: 'Sources', route: '/admin/sources' }]
        : []),
      ...(this.auth.has(Permissions.StatsView)
        ? [{ id: 'admin-stats', label: 'Statistics', route: '/admin/stats' }]
        : []),
    ];

    if (adminChildren.length) {
      items.push({ id: 'admin', label: 'Admin', icon: '⊗', children: adminChildren });
    }

    return items;
  });

  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map((e) => e.urlAfterRedirects),
      startWith(this.router.url),
    ),
    { initialValue: this.router.url },
  );

  protected readonly activeId = computed(() => {
    const url = this.currentUrl().split('?')[0];
    if (url.startsWith('/allnews') || url.startsWith('/feed') || url.startsWith('/article')) return 'allnews';
    if (url.startsWith('/foryou')) return 'foryou';
    if (url.startsWith('/trending')) return 'trending';
    if (url.startsWith('/subscriptions')) return 'subscriptions';
    if (url.startsWith('/saved')) return 'saved';
    if (url.startsWith('/admin/sources')) return 'admin-sources';
    if (url.startsWith('/admin/stats')) return 'admin-stats';
    if (url.startsWith('/admin')) return 'admin-sources';
    return null;
  });
}
