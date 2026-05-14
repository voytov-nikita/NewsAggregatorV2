import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { Store } from '@ngrx/store';
import { toSignal } from '@angular/core/rxjs-interop';
import { UiActions, selectSidebarCollapsed } from '../../../store';
import { ThemeService } from '../../services/theme.service';
import { NavItem } from './sidebar.types';

@Component({
  standalone: false,
  selector: 'app-sidebar',
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Sidebar {
  private readonly store = inject(Store);
  protected readonly themeService = inject(ThemeService);

  readonly items = input.required<readonly NavItem[]>();
  readonly activeId = input<string | null>(null);

  protected readonly collapsed = toSignal(this.store.select(selectSidebarCollapsed), {
    initialValue: false,
  });

  protected readonly themeIcon = computed(() => (this.themeService.theme() === 'dark' ? '☀' : '◑'));
  protected readonly themeTitle = computed(() =>
    this.themeService.theme() === 'dark' ? 'Switch to Light mode' : 'Switch to Dark mode',
  );

  protected toggleCollapsed(): void {
    this.store.dispatch(UiActions.toggleSidebar());
  }

  protected toggleTheme(): void {
    this.themeService.toggle();
  }

  protected isItemActive(item: NavItem): boolean {
    const active = this.activeId();
    if (!active) return false;
    if (item.children?.length) return active.startsWith(item.id);
    return active === item.id;
  }

  protected isChildActive(childId: string): boolean {
    return this.activeId() === childId;
  }
}
