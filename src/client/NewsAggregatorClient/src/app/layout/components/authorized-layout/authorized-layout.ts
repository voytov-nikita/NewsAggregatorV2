import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map, startWith } from 'rxjs';
import { NavItem } from '@shared/components';
import { NavigationService } from '@shared/services';

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

  protected readonly navItems: NavItem[] = [
    { id: 'feed', label: 'Feed', icon: '⊞', route: '/feed' },
    { id: 'saved', label: 'Saved', icon: '⊕', comingSoon: true },
    {
      id: 'admin',
      label: 'Admin',
      icon: '⊗',
      children: [
        { id: 'admin-sources', label: 'Sources', route: '/admin/sources' },
        { id: 'admin-stats', label: 'Statistics', route: '/admin/stats' },
      ],
    },
  ];

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
    if (url.startsWith('/feed') || url.startsWith('/article')) return 'feed';
    if (url.startsWith('/admin/sources')) return 'admin-sources';
    if (url.startsWith('/admin/stats')) return 'admin-stats';
    if (url.startsWith('/admin')) return 'admin-sources';
    return null;
  });
}
