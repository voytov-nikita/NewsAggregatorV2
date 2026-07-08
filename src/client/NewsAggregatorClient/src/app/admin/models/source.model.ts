export type SourceType = 'Feed' | 'Html';

export type NewsCategory =
  | 'Uncategorized'
  | 'Technology'
  | 'Architecture'
  | 'Backend'
  | 'Frontend'
  | 'Database'
  | 'DevOps'
  | 'RealTime'
  | 'State';

export const NEWS_CATEGORIES: NewsCategory[] = [
  'Uncategorized',
  'Technology',
  'Architecture',
  'Backend',
  'Frontend',
  'Database',
  'DevOps',
  'RealTime',
  'State',
];

export interface FeedFieldMapping {
  titlePath?: string;
  descriptionPath?: string;
  imagePath?: string;
  dateTimePath?: string;
  publisherPath?: string;
  linkPath?: string;
  guidPath?: string;
}

export interface HtmlSelectors {
  titleSelector?: string;
  descriptionSelector?: string;
  imageSelector?: string;
  dateTimeSelector?: string;
  publisherSelector?: string;
  linkSelector?: string;
}

export interface Source {
  id: string;
  name: string;
  publisherName: string;
  publisherLink: string;
  url: string;
  type: SourceType;
  cronSchedule: string;
  encoding?: string | null;
  enabled: boolean;
  category: NewsCategory;
  feedMapping?: FeedFieldMapping | null;
  htmlSelectors?: HtmlSelectors | null;

  lastCrawlAt?: string | null;
  lastCrawlSuccess?: boolean | null;
  lastCrawlDurationMs?: number;
  lastParsedCount?: number;
  articlesCount?: number;
  consecutiveFailures?: number;
}

export type FieldKey = 'title' | 'description' | 'image' | 'dateTime' | 'publisher';

export interface FieldTarget {
  key: FieldKey;
  label: string;
  desc: string;
}

export interface SamplePath {
  path: string;
  label: string;
  value: string;
}
