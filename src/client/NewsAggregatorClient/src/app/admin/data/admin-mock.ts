import { FieldKey, FieldTarget, SamplePath, Source, SourceType } from '../models/source.model';

const SEEDS = [
  { name: 'Angular Blog', articles: 142, success: 98, avgDelay: 1.2, lastCrawl: '2m ago', type: 'RSS' as SourceType },
  { name: 'InfoQ', articles: 89, success: 94, avgDelay: 2.8, lastCrawl: '5m ago', type: 'RSS' as SourceType },
  { name: 'The New Stack', articles: 201, success: 99, avgDelay: 0.9, lastCrawl: '1m ago', type: 'RSS' as SourceType },
  { name: 'Postgres Weekly', articles: 56, success: 100, avgDelay: 0.6, lastCrawl: '8m ago', type: 'RSS' as SourceType },
  { name: 'CSS-Tricks', articles: 34, success: 91, avgDelay: 3.4, lastCrawl: '12m ago', type: 'HTML' as SourceType },
  { name: 'ngrx team', articles: 78, success: 97, avgDelay: 1.8, lastCrawl: '3m ago', type: 'RSS' as SourceType },
  { name: 'Particular Software', articles: 45, success: 88, avgDelay: 4.1, lastCrawl: '18m ago', type: 'HTML' as SourceType },
  { name: 'MongoDB Blog', articles: 67, success: 96, avgDelay: 2.2, lastCrawl: '6m ago', type: 'RSS' as SourceType },
];

const FREQ = [15, 30, 60, 5, 30, 15, 60, 30];
const POLICY = ['OnFailure', 'Always', 'OnFailure', 'Never', 'OnFailure', 'Always', 'OnFailure', 'Always'] as const;

export const MOCK_SOURCES: Source[] = SEEDS.map((s, i) => ({
  ...s,
  id: i + 1,
  url: `https://${s.name.toLowerCase().replace(/\s+/g, '-')}.com/feed`,
  active: s.success > 90,
  frequency: FREQ[i],
  restartPolicy: POLICY[i],
  timeout: 10,
  retries: 3,
}));

export const FIELD_TARGETS: FieldTarget[] = [
  { key: 'title', label: 'Title', desc: 'Article headline' },
  { key: 'description', label: 'Description', desc: 'Summary / body text' },
  { key: 'image', label: 'Image URL', desc: 'Thumbnail or hero' },
  { key: 'dateTime', label: 'Date / Time', desc: 'Publication timestamp' },
  { key: 'publisher', label: 'Publisher', desc: 'Author or byline' },
];

export const RSS_SAMPLE: SamplePath[] = [
  { path: 'item/title', label: 'title', value: 'ASP.NET Core 9 Performance Benchmarks: 40% Faster' },
  { path: 'item/description', label: 'description', value: 'We look at what changed under the hood and how it affects...' },
  { path: 'item/pubDate', label: 'pubDate', value: 'Mon, 28 Apr 2026 09:15:00 GMT' },
  { path: 'item/link', label: 'link', value: 'https://devblog.example.com/aspnet-core-9' },
  { path: 'item/enclosure/@url', label: 'enclosure[url]', value: 'https://devblog.example.com/img/aspnet9.jpg' },
  { path: 'item/author', label: 'author', value: 'John Smith' },
  { path: 'item/category', label: 'category', value: 'Performance' },
  { path: 'item/guid', label: 'guid', value: 'tag:devblog.example.com,2026:aspnet-core-9' },
];

export const HTML_SAMPLE: SamplePath[] = [
  { path: 'article/h1', label: 'h1', value: 'ASP.NET Core 9 Performance Benchmarks' },
  { path: 'article/.post-meta/time', label: 'time[datetime]', value: '2026-04-28T09:15:00Z' },
  { path: 'article/.post-excerpt > p', label: '.post-excerpt > p', value: 'We look at what changed under the hood...' },
  { path: 'article/.author-name', label: '.author-name', value: 'John Smith' },
  { path: 'article/.hero-img/@src', label: '.hero-img[src]', value: '/img/aspnet9-hero.jpg' },
  { path: 'article/.tag-list/a:first', label: '.tag-list a:first', value: 'Performance' },
  { path: 'article/meta[og:title]', label: 'meta[og:title]', value: 'ASP.NET Core 9 Performance Benchmarks' },
  { path: 'article/meta[og:image]', label: 'meta[og:image]', value: 'https://devblog.example.com/img/aspnet9.jpg' },
];

export const AUTO_MAPPINGS: Record<SourceType, Partial<Record<FieldKey, string>>> = {
  RSS: {
    title: 'item/title',
    description: 'item/description',
    image: 'item/enclosure/@url',
    dateTime: 'item/pubDate',
    publisher: 'item/author',
  },
  HTML: {
    title: 'article/h1',
    description: 'article/.post-excerpt > p',
    image: 'article/.hero-img/@src',
    dateTime: 'article/.post-meta/time',
    publisher: 'article/.author-name',
  },
};

export const FREQUENCY_OPTIONS: { label: string; value: number }[] = [
  { label: '10 seconds', value: 10 / 60 },
  { label: '30 seconds', value: 30 / 60 },
  { label: '1 minute', value: 1 },
  { label: '5 minutes', value: 5 },
  { label: '15 minutes', value: 15 },
  { label: '30 minutes', value: 30 },
  { label: '1 hour', value: 60 },
];

export const RESTART_POLICIES = ['OnFailure', 'Always', 'Never'] as const;
