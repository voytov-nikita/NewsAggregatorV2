import { Injectable, signal } from '@angular/core';

/**
 * Holds the access token in memory only.
 *
 * Deliberately not localStorage / sessionStorage: that is the whole point of keeping the refresh
 * token in an httpOnly cookie. A XSS can read any web storage, so a token parked there hands the
 * attacker the session; a token that only ever lives in a closure dies with the tab.
 */
@Injectable({ providedIn: 'root' })
export class TokenStore {
  private readonly _accessToken = signal<string | null>(null);

  public readonly accessToken = this._accessToken.asReadonly();

  public set(token: string): void {
    this._accessToken.set(token);
  }

  public clear(): void {
    this._accessToken.set(null);
  }
}
