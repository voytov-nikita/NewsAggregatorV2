using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.DAL.Entities;
using CrawlerService.Models.Models;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;

namespace CrawlerService.DAL.IntegrationTests.Stores;

public class NewsStoreTests : BaseTest
{
    private readonly INewsStore _sut;
    private readonly Fixture _fixture;

    public NewsStoreTests(EnvironmentFixture fixture)
        : base(fixture)
    {
        _fixture = new Fixture();

        _sut = GetService<INewsStore>();
    }

    [Fact]
    public async Task InsertAsync_NotEmptyData_SuccessInserted()
    {
        // Arrange
        CustomizeParsedNews();

        var parsedNews = _fixture.CreateMany<ParsedNews>(10)
            .ToArray();

        // Act
        Func<Task> act = () => _sut.InsertAsync(parsedNews);

        // Assert
        await act
            .Should()
            .NotThrowAsync<Exception>();

        await VerifyNewsInserted(parsedNews);
    }

    [Fact]
    public async Task ExcludeExistedAsync_ExistedGlobalIds_EmptyGlobalIdsArray()
    {
        // Arrange
        CustomizeRawNewsEntity();

        var rawNewsEntities = _fixture.CreateMany<RawNewsEntity>(10)
            .ToArray();

        var rawNewsEntitiesIds = rawNewsEntities
            .Select(_ => _.GlobalUniqueId)
            .ToArray();

        await ExecuteDbOperationAsync(_ =>
            _.GetCollection<RawNewsEntity>("rawNews")
                .InsertManyAsync(rawNewsEntities)
        );
        
        // Actual
        string[] actual = await _sut.ExcludeExistedAsync(rawNewsEntitiesIds);

        // Assert
        actual.Should()
            .BeEmpty();
    }

    [Fact]
    public async Task ExcludeExistedAsync_NotExistedGlobalIds_NotEmptyGlobalIdsArray()
    {
        // Arrange
        var rawNewsEntitiesIds = _fixture
            .CreateMany<Guid>(10)
            .Select(_ => _.ToString())
            .ToArray();

        // Actual
        string[] actual = await _sut.ExcludeExistedAsync(rawNewsEntitiesIds);

        // Assert
        actual.Should()
            .BeEquivalentTo(rawNewsEntitiesIds);
    }

    #region Private methods

    private async Task VerifyNewsInserted(ParsedNews[] parsedNews)
    {
        var parsedNewsGuids = parsedNews
            .Select(n => n.GlobalUniqueId)
            .ToArray();

        List<RawNewsEntity> newsEntities = await ExecuteDbOperationAsync(_ =>
        {
            return _.GetCollection<RawNewsEntity>("rawNews")
                .Find(e => parsedNewsGuids.Contains(e.GlobalUniqueId)).ToListAsync();
        });

        newsEntities.Should()
            .NotBeNullOrEmpty();

        var readNewsIds = newsEntities
            .Select(_ => _.GlobalUniqueId)
            .ToHashSet()
            .ToArray();

        var insertedNewsIds = parsedNews
            .Select(_ => _.GlobalUniqueId)
            .ToArray();

        readNewsIds.Should()
            .BeEquivalentTo(insertedNewsIds);
    }
    
    private void CustomizeParsedNews()
    {
        _fixture.Customize<ParsedNews>(_ =>
            _.With(x => x.GlobalUniqueId, () => Guid.NewGuid().ToString())
        );
    }
    private void CustomizeRawNewsEntity()
    {
        _fixture.Customize<RawNewsEntity>(_ =>
            _
                .With(x => x.GlobalUniqueId, () => Guid.NewGuid().ToString())
                .Without(x => x.Id)
        );
    }
    
    #endregion
}