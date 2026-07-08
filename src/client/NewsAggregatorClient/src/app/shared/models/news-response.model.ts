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

export interface NewsResponse {
  id: number;
  title: string;
  description: string;
  originalLink: string;
  publishDate: string;
  readDate: string;
  imageLink: string | null;
  publisher: string;
  publisherLink: string;

  category: NewsCategory;
  tags: string[];
  readTimeMinutes: number;
  likes: number;
  dislikes: number;
  commentsCount: number;
}
