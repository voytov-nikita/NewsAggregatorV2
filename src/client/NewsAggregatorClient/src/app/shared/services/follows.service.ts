import { Injectable, computed, signal } from '@angular/core';

export type FollowKind = 'categories' | 'sources';

interface FollowsState {
  categories: ReadonlySet<string>;
  sources: ReadonlySet<string>;
}

const STORAGE_KEY = 'na-follows';

@Injectable({ providedIn: 'root' })
export class FollowsService {
  private readonly _state = signal<FollowsState>(this.load());

  public readonly state = computed(() => this._state());
  public readonly hasAny = computed(() => {
    const s = this._state();
    return s.categories.size + s.sources.size > 0;
  });

  public list(kind: FollowKind): ReadonlySet<string> {
    return this._state()[kind];
  }

  public isFollowing(kind: FollowKind, key: string): boolean {
    return this._state()[kind].has(key);
  }

  public toggle(kind: FollowKind, key: string): boolean {
    const current = this._state();
    const next = new Set(current[kind]);
    const isNowFollowing = !next.has(key);
    if (isNowFollowing) next.add(key);
    else next.delete(key);
    const updated = { ...current, [kind]: next };
    this._state.set(updated);
    this.persist(updated);
    return isNowFollowing;
  }

  public setMany(kind: FollowKind, keys: Iterable<string>): void {
    const current = this._state();
    const updated = { ...current, [kind]: new Set(keys) };
    this._state.set(updated);
    this.persist(updated);
  }

  private load(): FollowsState {
    const empty: FollowsState = { categories: new Set(), sources: new Set() };
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return empty;
      const parsed = JSON.parse(raw) as Partial<Record<FollowKind, string[]>>;
      return {
        categories: new Set(parsed.categories ?? []),
        sources: new Set(parsed.sources ?? []),
      };
    } catch {
      return empty;
    }
  }

  private persist(state: FollowsState): void {
    try {
      localStorage.setItem(
        STORAGE_KEY,
        JSON.stringify({
          categories: [...state.categories],
          sources: [...state.sources],
        }),
      );
    } catch {
      // ignore
    }
  }
}
