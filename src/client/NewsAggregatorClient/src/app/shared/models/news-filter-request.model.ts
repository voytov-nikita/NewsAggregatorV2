import { NewsOrderField, OrderDirection } from '@shared/enums';

export interface NewsFilterRequest {
  take: number;
  offset: number;
  keyword?: string;
  orderBy: NewsOrderField;
  orderDirection: OrderDirection;
}
