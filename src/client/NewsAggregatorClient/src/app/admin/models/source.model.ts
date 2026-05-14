export type SourceType = 'RSS' | 'HTML';
export type RestartPolicy = 'OnFailure' | 'Always' | 'Never';

export interface Source {
  id: number;
  name: string;
  url: string;
  type: SourceType;
  active: boolean;
  articles: number;
  success: number;
  avgDelay: number;
  lastCrawl: string;
  frequency: number;
  restartPolicy: RestartPolicy;
  timeout: number;
  retries: number;
  fieldMapping?: Partial<Record<FieldKey, string>>;
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
