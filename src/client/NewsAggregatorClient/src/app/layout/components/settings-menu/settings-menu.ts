import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map, startWith } from 'rxjs';
import { NavigationService } from '@shared/services';

interface SettingsItem {
  id: string;
  label: string;
}

const SETTINGS_MENU: SettingsItem[] = [
  { id: 'account', label: 'Account' },
  { id: 'feed', label: 'Feed Preferences' },
  { id: 'categories', label: 'Categories' },
  { id: 'notifications', label: 'Notifications' },
  { id: 'privacy', label: 'Privacy' },
  { id: 'statistics', label: 'Statistics' },
];

@Component({
  standalone: false,
  selector: 'app-settings-menu',
  templateUrl: './settings-menu.html',
  styleUrl: './settings-menu.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SettingsMenu {
  private readonly router = inject(Router);
  private readonly navigation = inject(NavigationService);

  protected readonly items = SETTINGS_MENU;
  protected readonly open = signal(false);

  protected readonly url = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map((e) => e.urlAfterRedirects),
      startWith(this.router.url),
    ),
    { initialValue: this.router.url },
  );

  protected readonly onSettings = computed(() => this.url().startsWith('/settings'));

  protected readonly currentSection = computed(() => {
    const url = this.url();
    if (!url.startsWith('/settings')) return null;
    const segments = url.split('?')[0].split('/').filter(Boolean);
    return segments[1] ?? 'account';
  });

  protected toggle(): void {
    this.open.update((v) => !v);
  }

  protected close(): void {
    this.open.set(false);
  }

  protected go(item: SettingsItem): void {
    this.navigation.toSettings(item.id);
    this.close();
  }
}
