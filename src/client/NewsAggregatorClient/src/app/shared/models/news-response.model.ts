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
}
