import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FollowKind, FollowsService, ToastService } from '@shared/services';
import { CATEGORIES, KNOWN_SOURCES } from '../../../shared/data/news-catalog';

interface TabDef {
  id: FollowKind;
  label: string;
}

interface Row {
  key: string;
  label: string;
  meta?: string;
  following: boolean;
}

const TABS: TabDef[] = [
  { id: 'categories', label: 'Categories' },
  { id: 'sources', label: 'Sources' },
];

@Component({
  standalone: false,
  selector: 'app-subscriptions',
  templateUrl: './subscriptions.html',
  styleUrl: './subscriptions.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Subscriptions {
  private readonly follows = inject(FollowsService);
  private readonly toast = inject(ToastService);

  protected readonly tabs = TABS;
  protected readonly activeTab = signal<FollowKind>('categories');
  protected readonly query = signal('');

  protected readonly rows = computed<Row[]>(() => {
    const tab = this.activeTab();
    const state = this.follows.state();
    const q = this.query().trim().toLowerCase();

    let base: Row[];
    switch (tab) {
      case 'categories':
        base = CATEGORIES.map((c) => ({
          key: c,
          label: c,
          following: state.categories.has(c),
        }));
        break;
      case 'sources':
        base = KNOWN_SOURCES.map((s) => ({
          key: s,
          label: s,
          following: state.sources.has(s),
        }));
        break;
    }
    if (!q) return base;
    return base.filter((r) => r.label.toLowerCase().includes(q));
  });

  protected readonly followingCount = computed(() => {
    const s = this.follows.state();
    return s.categories.size + s.sources.size;
  });

  protected setTab(id: FollowKind): void {
    this.activeTab.set(id);
    this.query.set('');
  }

  protected onSearch(value: string): void {
    this.query.set(value);
  }

  protected toggle(row: Row): void {
    const tab = this.activeTab();
    const isFollowing = this.follows.toggle(tab, row.key);
    this.toast.info(isFollowing ? `Following ${row.label}` : `Unfollowed ${row.label}`);
  }
}
