import { NewsResponse } from '@shared/models';

// Canonical taxonomy — mirrors NewsService.Models.News.Enum.NewsCategory (server enum).
// Server sends category names as strings via JsonStringEnumConverter.
export const CATEGORIES = [
  'Technology',
  'Architecture',
  'Backend',
  'Frontend',
  'Database',
  'DevOps',
  'RealTime',
  'State',
] as const;

export type Category = (typeof CATEGORIES)[number];

// Source: prototype/na-shared.jsx — OKLCH hue rotation per category.
export const CATEGORY_HUES: Record<Category, number> = {
  Technology: 220,
  Architecture: 160,
  Backend: 30,
  Frontend: 280,
  Database: 200,
  DevOps: 120,
  RealTime: 10,
  State: 250,
};

// Human-friendly labels for UI; enum names stay PascalCase for transport.
export const CATEGORY_LABELS: Record<Category, string> = {
  Technology: 'Technology',
  Architecture: 'Architecture',
  Backend: 'Backend',
  Frontend: 'Frontend',
  Database: 'Database',
  DevOps: 'DevOps',
  RealTime: 'Real-time',
  State: 'State',
};

// Known sources still useful for the Subscriptions screen; categorization itself
// is now server-driven via NewsResponse.category.
export const KNOWN_SOURCES = [
  'Angular Blog',
  'CSS-Tricks',
  'ngrx team',
  'InfoQ',
  'The New Stack',
  'Postgres Weekly',
  'Microsoft Dev Blog',
  'Particular Software',
  'MongoDB Blog',
];

// Category is server-authoritative. Returns the raw value (including
// 'Uncategorized') — SourceBadge hides the chip for Uncategorized articles.
export function categoryFor(article: Pick<NewsResponse, 'category'>): string {
  return article.category ?? 'Uncategorized';
}

export function hueFor(category: Category): number {
  return CATEGORY_HUES[category] ?? 220;
}

export function readTimeFor(article: Pick<NewsResponse, 'readTimeMinutes' | 'description'>): number {
  if (article.readTimeMinutes && article.readTimeMinutes > 0) return article.readTimeMinutes;
  const len = article.description?.length ?? 600;
  return Math.max(3, Math.round(len / 200));
}
