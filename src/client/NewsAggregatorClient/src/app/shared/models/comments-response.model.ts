export interface CommentsResponse {
  id: number;
  content: string;
  createDate: string;
  lastModifiedDate: string | null;
  likes: number;
  disLikes: number;
}
