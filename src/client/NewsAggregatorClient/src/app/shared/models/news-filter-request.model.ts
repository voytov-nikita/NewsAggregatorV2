import { NewsOrderField, OrderDirection } from '@shared/enums';
import { NewsCategory } from './news-response.model';

export interface NewsFilterRequest {
  take: number;
  offset: number;
  keyword?: string;
  orderBy: NewsOrderField;
  orderDirection: OrderDirection;

  /** Single-category convenience (mirrors chip UI). Serialised as one `categories` value. */
  category?: NewsCategory;

  categories?: NewsCategory[];
  sources?: string[];
  dateFrom?: string;
  dateTo?: string;
}
