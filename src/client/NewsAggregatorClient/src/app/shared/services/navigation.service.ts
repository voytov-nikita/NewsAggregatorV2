import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  private readonly router = inject(Router);

  private readonly feedRoute: string[] = ['/allnews'];
  private readonly settingsRoute: string[] = ['/settings'];
  private readonly adminSourcesRoute: string[] = ['/admin', 'sources'];
  private readonly adminStatsRoute: string[] = ['/admin', 'stats'];

  public getFeedRoute(): string[] {
    return this.feedRoute;
  }

  public toFeed(): void {
    this.router.navigate(this.feedRoute);
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

  /**
   * Opens the article's source URL in a new tab. The internal /article/:id
   * discussion screen was removed — the parser doesn't reliably extract the
   * article body, so we send the reader to the original source instead.
   */
  public openOriginal(article: { originalLink: string }): void {
    if (!article?.originalLink) return;
    window.open(article.originalLink, '_blank', 'noopener,noreferrer');
  }

  // Legacy alias — kept until callers migrate.
  public getNewsListRoute(): string[] {
    return this.feedRoute;
  }
  public toNewsListRoute(): void {
    this.toFeed();
  }
}
