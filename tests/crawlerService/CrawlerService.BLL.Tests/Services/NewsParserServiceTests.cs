using System.Linq.Expressions;
using System.Xml;
using AutoFixture;
using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.BLL.Services;
using CrawlerService.Models.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CrawlerService.BLL.Tests.Services;

public class NewsParserServiceTests
{
    private readonly NewsParserService _sut;
    private readonly Mock<IPostponedJobRunner> _postponedJobRunnerMock = new();
    private readonly Fixture _fixture = new Fixture();

    public NewsParserServiceTests()
    {
        _sut = new NewsParserService(_postponedJobRunnerMock.Object, NullLogger<NewsParserService>.Instance);
    }

    [Fact]
    public void ParseAsync_ValidData_NoExceptionThrown()
    {
        // Arrange
        ParsedNews[] parsedNews = _fixture.CreateMany<ParsedNews>().ToArray();
        byte[] byteArray = GenerateXmlByteArray(parsedNews);

        // Act
        Func<Task> act = () => _sut.ParseAsync(byteArray);

        // Assert
        act.Should()
            .NotThrowAsync();
        
        //Todo: Check that Enqueue method called SaveUniqueNewsAsync method with parsedNews as a parameter
        _postponedJobRunnerMock.Verify(_ => _.Enqueue(It.IsAny<Expression<Func<INewsService, Task>>>()), Times.Once);
    }
    
    
    [Fact]
    public void ParseAsync_NotValidData_ExceptionThrown()
    {
        // Arrange
        byte[] byteArray = GenerateNotValidXmlByteArray();

        // Act
        Func<Task> act = () => _sut.ParseAsync(byteArray);

        // Assert
        act.Should()
            .ThrowAsync<Exception>();
        
        _postponedJobRunnerMock.Verify(_ => _.Enqueue(It.IsAny<Expression<Func<INewsService, Task>>>()), Times.Never);
    }

    #region PrivateMethods

    private byte[] GenerateXmlByteArray(ParsedNews[]? parsedNews = null)
    {
        var xmlDocument = new XmlDocument();

        parsedNews ??= _fixture.CreateMany<ParsedNews>().ToArray();

        xmlDocument.LoadXml(
            $"""
             <rss xmlns:content="http://purl.org/rss/1.0/modules/content/" xmlns:dc="http://purl.org/dc/elements/1.1/" version="2.0">
                 <channel>
                     {
                         string.Join("", parsedNews.Select(news => $"""
                                                                        <item>
                                                                            <title>{news.Title}</title>
                                                                            <description>{news.Description}</description>
                                                                            <category>Новини</category>
                                                                            <link>{news.OriginalLink}</link>
                                                                            <enclosure url="{news.ImageLink}" />
                                                                            <pubDate>{news.PublishDate:ddd, dd MMM yyyy HH:mm:ss} GMT</pubDate>
                                                                            <dc:creator>{news.PublisherName}</dc:creator>
                                                                            <guid>{news.Guid}</guid>
                                                                        </item>
                                                                    """))
                     }
                 </channel>
             </rss>
             """
        );

        using (var memoryStream = new MemoryStream())
        {
            xmlDocument.Save(memoryStream);
            return memoryStream.ToArray();
        }
    }

    private byte[] GenerateNotValidXmlByteArray()
    {
        return _fixture.CreateMany<byte>(100).ToArray();
    }

    #endregion
}