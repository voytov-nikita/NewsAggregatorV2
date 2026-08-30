/**
 * Mirrors Common.Auth/Constants/Permissions.cs on the server. The server is the authority: this
 * copy only decides what the UI shows, never what the API allows.
 */
export const Permissions = {
  NewsVote: 'news.vote',
  CommentWrite: 'comment.write',
  CommentModerate: 'comment.moderate',
  SourcesManage: 'sources.manage',
  StatsView: 'stats.view',
  WebhooksManage: 'webhooks.manage',
} as const;

export type Permission = (typeof Permissions)[keyof typeof Permissions];
