namespace Common.Models;

/// <summary>
/// Canonical taxonomy for aggregated news. Shared across services so the
/// crawler, the message queue and the news service all speak the same enum.
/// Values are persisted as <c>short</c>; never renumber existing entries
/// — additions only.
/// </summary>
public enum NewsCategory : short
{
    Uncategorized = 0,
    Technology    = 1,
    Architecture  = 2,
    Backend       = 3,
    Frontend      = 4,
    Database      = 5,
    DevOps        = 6,
    RealTime      = 7,
    State         = 8,
}
