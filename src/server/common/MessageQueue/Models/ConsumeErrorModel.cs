namespace MessageQueue.Models;

internal class ConsumeErrorModel
{
	public string OriginalQueue { get; set; } = null!;

	public byte[] OrigimalMeggage { get; set; } = null!;

	public string? ExceptionTypeName { get; set; }

	public string ExceptionMessage { get; set; }
}
