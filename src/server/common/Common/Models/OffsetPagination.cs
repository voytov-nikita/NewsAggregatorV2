namespace Common.Models;

public class OffsetPagination
{
	public OffsetPagination(int offset, int take)
	{
		Offset = offset;
		Take = take;
	}

	public int Offset { get; }

	public int Take { get; }

	/// <summary>
	/// A default pagination object to be used when a pagination object is not specified.
	/// </summary>
	public static OffsetPagination Default { get; } = new OffsetPagination(0, 100);

	/// <summary>
	/// A pagination object to be used when first object should be returned.
	/// </summary>
	public static OffsetPagination First { get; } = new OffsetPagination(0, 1);

	/// <summary>
	/// A pagination object to be used when all objects should be returned.
	/// </summary>
	public static OffsetPagination None { get; } = new OffsetPagination(0, int.MaxValue);
}
