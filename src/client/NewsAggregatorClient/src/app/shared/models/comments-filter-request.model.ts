import { CommentsOrderField, OrderDirection } from '@shared/enums';

export interface CommentsFilterRequest {
  take: number;
  offset: number;
  orderBy: CommentsOrderField;
  orderDirection: OrderDirection;
}
