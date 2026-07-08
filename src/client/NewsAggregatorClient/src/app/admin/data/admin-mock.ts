import { FieldKey, FieldTarget, SamplePath, SourceType } from '../models/source.model';

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
  Feed: {
    title: 'item/title',
    description: 'item/description',
    image: 'item/enclosure/@url',
    dateTime: 'item/pubDate',
    publisher: 'item/author',
  },
  Html: {
    title: 'article/h1',
    description: 'article/.post-excerpt > p',
    image: 'article/.hero-img/@src',
    dateTime: 'article/.post-meta/time',
    publisher: 'article/.author-name',
  },
};
