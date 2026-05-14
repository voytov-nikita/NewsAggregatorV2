import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  private readonly router = inject(Router);

  private readonly feedRoute: string[] = ['/feed'];
  private readonly articleRoute: string[] = ['/article'];
  private readonly settingsRoute: string[] = ['/settings'];
  private readonly adminSourcesRoute: string[] = ['/admin', 'sources'];
  private readonly adminStatsRoute: string[] = ['/admin', 'stats'];

  public getFeedRoute(): string[] {
    return this.feedRoute;
  }

  public toFeed(): void {
    this.router.navigate(this.feedRoute);
  }

  public getArticleRoute(id: string | number): string[] {
    return [...this.articleRoute, id.toString()];
  }

  public toArticle(id: string | number): void {
    this.router.navigate(this.getArticleRoute(id));
  }

  public getSettingsRoute(section?: string): string[] {
    return section ? [...this.settingsRoute, section] : this.settingsRoute;
  }

  public toSettings(section?: string): void {
    this.router.navigate(this.getSettingsRoute(section));
  }

  public getAdminSourcesRoute(): string[] {
    return this.adminSourcesRoute;
  }

  public getAdminStatsRoute(): string[] {
    return this.adminStatsRoute;
  }

  public toDefaultPage(): void {
    this.router.navigate(this.feedRoute);
  }

  // Legacy aliases — kept until callers migrate.
  public getNewsListRoute(): string[] {
    return this.feedRoute;
  }
  public toNewsListRoute(): void {
    this.toFeed();
  }
  public getNewsDetailRoute(id: string | number): string[] {
    return this.getArticleRoute(id);
  }
  public toNewsDetailRoute(id: string | number): void {
    this.toArticle(id);
  }
}
