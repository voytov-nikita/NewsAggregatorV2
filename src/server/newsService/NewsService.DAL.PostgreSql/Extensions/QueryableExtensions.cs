using System.Linq.Expressions;
using Common.Enums;

namespace NewsService.DAL.PostgreSql.Extensions;


public static class QueryableExtensions
{
    public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        OrderDirection orderDirection)
    {
        return orderDirection == OrderDirection.Ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
    }

    public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(
        this IOrderedQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        OrderDirection orderDirection)
    {
        return orderDirection == OrderDirection.Ascending ? source.ThenBy(keySelector) : source.ThenByDescending(keySelector);
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        OrderDirection orderDirection)
    {
        return orderDirection == OrderDirection.Ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
    }

    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
        this IOrderedEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        OrderDirection orderDirection)
    {
        return orderDirection == OrderDirection.Ascending ? source.ThenBy(keySelector) : source.ThenByDescending(keySelector);
    }
}