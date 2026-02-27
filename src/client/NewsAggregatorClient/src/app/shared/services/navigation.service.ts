import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  private newsListRoute: string[] = ['/'];
  private newsDetailRoute: string[] = ['/news'];
  private settingsRoute: string[] = ['/settings'];

  constructor(private router: Router) {}

  public getNewsListRoute(): string[] {
    return this.newsListRoute;
  }

  public toNewsListRoute(): void {
    this.router.navigate(this.newsListRoute);
  }

  public getNewsDetailRoute(id: string | number): string[] {
    return [...this.newsDetailRoute, id.toString()];
  }

  public toNewsDetailRoute(id: string | number): void {
    this.router.navigate(this.getNewsDetailRoute(id));
  }

  public getSettingsRoute(): string[] {
    return this.settingsRoute;
  }

  public toSettingsRoute(): void {
    this.router.navigate(this.settingsRoute);
  }

  public toDefaultPage(): void {
    this.router.navigate(this.newsListRoute);
  }
}
