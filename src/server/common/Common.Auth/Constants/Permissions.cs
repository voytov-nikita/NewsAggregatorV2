namespace Common.Auth.Constants;

/// <summary>
/// The permission catalogue. This is the source of truth: AuthService seeds its Permissions table
/// from <see cref="All"/>, and services reference these constants in [HasPermission].
/// </summary>
public static class Permissions
{
    public const string NewsVote = "news.vote";

    public const string CommentWrite = "comment.write";

    public const string CommentModerate = "comment.moderate";

    public const string SourcesManage = "sources.manage";

    public const string StatsView = "stats.view";

    public const string WebhooksManage = "webhooks.manage";

    public static readonly string[] All =
    [
        NewsVote,
        CommentWrite,
        CommentModerate,
        SourcesManage,
        StatsView,
        WebhooksManage,
    ];
}
