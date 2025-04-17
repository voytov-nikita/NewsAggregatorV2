using System.Collections.ObjectModel;

namespace Common.Models;

/// <summary>
/// Represents a collection to be returned by methods leveraging offset-based pagination.
/// </summary>
public class OffsetCollection<T>: ReadOnlyCollection<T>
{
	public OffsetCollection(IList<T> list, int offset, int totalCount)
		: base(list)
	{
		Offset = offset;
		TotalCount = totalCount;
	}

	public int Offset { get; }

	public int TotalCount { get; }
}
