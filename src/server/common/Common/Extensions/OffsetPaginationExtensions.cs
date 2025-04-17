namespace Common.Models;

public static class OffsetPaginationExtensions
{
	public static IQueryable<T> ApplyPager<T>(this IQueryable<T> query, OffsetPagination pager)
	{
		return query.Skip(pager.Offset)
			.Take(pager.Take);
	}
}
