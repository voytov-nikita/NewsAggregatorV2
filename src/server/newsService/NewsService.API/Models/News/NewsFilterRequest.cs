namespace NewsService.API.Models.News;

public class NewsFilterRequest
{
    public int Take { get; set; }
    public int Offset { get; set; }
    public string OrderBy { get; set; }
    public string OrderDirection { get; set; }
    public string? Keyword { get; set; }
}