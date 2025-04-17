namespace Common.Models;

public static class OffsetCollectionExtensions
{
	public static OffsetCollection<T> ToOffsetCollection<T>(this IEnumerable<T> enumerable, int offset, int totalCount)
	{
		return new OffsetCollection<T>(enumerable.ToList(), offset, totalCount);
	}

	public static OffsetCollection<T> ToOffsetCollection<T>(this IEnumerable<T> enumerable, int offset)
	{
		var list = enumerable.ToList();
		return new OffsetCollection<T>(list, offset, list.Count);
	}
}
