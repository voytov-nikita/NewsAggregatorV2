import { Injectable, computed, signal } from '@angular/core';

const STORAGE_KEY = 'na-saved';

@Injectable({ providedIn: 'root' })
export class SavedArticlesService {
  private readonly _ids = signal<ReadonlySet<number>>(this.load());

  public readonly ids = computed(() => this._ids());
  public readonly count = computed(() => this._ids().size);

  public has(id: number): boolean {
    return this._ids().has(id);
  }

  public toggle(id: number): boolean {
    const next = new Set(this._ids());
    const isNowSaved = !next.has(id);
    if (isNowSaved) next.add(id);
    else next.delete(id);
    this._ids.set(next);
    this.persist(next);
    return isNowSaved;
  }

  public clear(): void {
    this._ids.set(new Set());
    this.persist(new Set());
  }

  private load(): Set<number> {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return new Set();
      const arr = JSON.parse(raw) as unknown;
      return Array.isArray(arr) ? new Set(arr.map(Number).filter((n) => !Number.isNaN(n))) : new Set();
    } catch {
      return new Set();
    }
  }

  private persist(set: Set<number>): void {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify([...set]));
    } catch {
      // ignore quota / private-mode errors
    }
  }
}
