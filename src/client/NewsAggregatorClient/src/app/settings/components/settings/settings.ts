import {
  AfterViewInit,
  Component,
  DestroyRef,
  ElementRef,
  inject,
  signal,
  viewChild,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { fromEvent } from 'rxjs';

interface Category {
  id: string;
  label: string;
  selected: boolean;
}

interface ToggleRow {
  title: string;
  description: string;
  active: boolean;
}

interface FavoriteSource {
  name: string;
  count: number;
  percentage: number;
}

interface SectionRef {
  id: string;
  label: string;
}

const SECTIONS: SectionRef[] = [
  { id: 'account', label: 'Account' },
  { id: 'feed', label: 'Feed Preferences' },
  { id: 'categories', label: 'Categories' },
  { id: 'notifications', label: 'Notifications' },
  { id: 'privacy', label: 'Privacy' },
  { id: 'statistics', label: 'Statistics' },
];

@Component({
  standalone: false,
  selector: 'app-settings',
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
})
export class Settings implements AfterViewInit {
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  protected readonly sections = SECTIONS;
  protected readonly activeSection = signal<string>('account');

  protected readonly displayName = signal('Nikita Vojtov');
  protected readonly email = signal('voytov.nikita@gmail.com');

  protected readonly density = signal<'comfortable' | 'compact'>('comfortable');
  protected readonly autoMarkRead = signal(true);
  protected readonly openInNewTab = signal(false);

  protected readonly categories = signal<Category[]>([
    { id: 'tech', label: 'Technology', selected: true },
    { id: 'arch', label: 'Architecture', selected: true },
    { id: 'be', label: 'Backend', selected: true },
    { id: 'fe', label: 'Frontend', selected: true },
    { id: 'db', label: 'Database', selected: false },
    { id: 'devops', label: 'DevOps', selected: false },
    { id: 'rt', label: 'Real-time', selected: true },
    { id: 'state', label: 'State', selected: false },
  ]);

  protected readonly notifications = signal<ToggleRow[]>([
    { title: 'New articles in favorite categories', description: 'Push when categories you follow get new content', active: true },
    { title: 'Trending', description: 'Most-discussed articles of the day', active: true },
    { title: 'Comment replies', description: 'Someone replied to your comment', active: true },
    { title: 'Weekly digest', description: 'Sunday summary of the week', active: false },
  ]);

  protected readonly privacy = signal<ToggleRow[]>([
    { title: 'Public profile', description: 'Others can view your profile', active: false },
    { title: 'Show activity', description: 'Display your reading history', active: true },
  ]);

  protected readonly stats = signal({ read: 247, saved: 18, comments: 52 });
  protected readonly favoriteSources = signal<FavoriteSource[]>([
    { name: 'Angular Blog', count: 72, percentage: 29 },
    { name: 'InfoQ', count: 54, percentage: 22 },
    { name: 'The New Stack', count: 41, percentage: 17 },
    { name: 'CSS-Tricks', count: 32, percentage: 13 },
  ]);

  private readonly content = viewChild.required<ElementRef<HTMLElement>>('content');

  ngAfterViewInit(): void {
    const initial = this.route.snapshot.paramMap.get('section');
    if (initial) {
      queueMicrotask(() => this.scrollTo(initial, 'auto'));
    }

    fromEvent(this.content().nativeElement, 'scroll', { passive: true })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.updateActiveFromScroll());
    this.updateActiveFromScroll();
  }

  protected scrollTo(id: string, behavior: ScrollBehavior = 'smooth'): void {
    const root = this.content().nativeElement;
    const target = root.querySelector<HTMLElement>(`[data-section="${id}"]`);
    if (!target) return;
    root.scrollTo({ top: target.offsetTop - 12, behavior });
    this.activeSection.set(id);
  }

  protected toggleCategory(id: string): void {
    this.categories.update((list) =>
      list.map((c) => (c.id === id ? { ...c, selected: !c.selected } : c)),
    );
  }

  protected toggleNotification(index: number): void {
    this.notifications.update((list) => {
      const next = [...list];
      next[index] = { ...next[index], active: !next[index].active };
      return next;
    });
  }

  protected togglePrivacy(index: number): void {
    this.privacy.update((list) => {
      const next = [...list];
      next[index] = { ...next[index], active: !next[index].active };
      return next;
    });
  }

  protected setDensity(value: 'comfortable' | 'compact'): void {
    this.density.set(value);
  }

  private updateActiveFromScroll(): void {
    const root = this.content().nativeElement;
    const top = root.scrollTop + 60;
    const sections = root.querySelectorAll<HTMLElement>('[data-section]');
    let current = this.sections[0].id;
    sections.forEach((el) => {
      if (el.offsetTop <= top) current = el.dataset['section'] ?? current;
    });
    if (current !== this.activeSection()) {
      this.activeSection.set(current);
    }
  }
}
