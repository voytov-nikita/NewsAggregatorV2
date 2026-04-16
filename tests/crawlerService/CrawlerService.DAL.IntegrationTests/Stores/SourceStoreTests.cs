using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;

namespace CrawlerService.DAL.IntegrationTests.Stores;

public class SourceStoreTests : BaseTest
{
    private const string CollectionName = "sources";

    private readonly ISourceStore _sut;
    private readonly Fixture _fixture = new();

    public SourceStoreTests(EnvironmentFixture fixture)
        : base(fixture)
    {
        _sut = GetService<ISourceStore>();
    }

    [Fact]
    public async Task UpsertAsync_NewSource_InsertsDocument()
    {
        NewsSource source = BuildSource("upsert-insert", enabled: true);

        await _sut.UpsertAsync(source);

        NewsSource? actual = await _sut.GetByIdAsync(source.Id);
        actual.Should().NotBeNull();
        actual!.Name.Should().Be(source.Name);
    }

    [Fact]
    public async Task UpsertAsync_ExistingSource_ReplacesDocument()
    {
        NewsSource source = BuildSource("upsert-update", enabled: true);
        await InsertDirectly(source);

        source.Name = "Updated Name";
        source.Enabled = false;
        await _sut.UpsertAsync(source);

        NewsSource? actual = await _sut.GetByIdAsync(source.Id);
        actual.Should().NotBeNull();
        actual!.Name.Should().Be("Updated Name");
        actual.Enabled.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        NewsSource? actual = await _sut.GetByIdAsync($"missing-{Guid.NewGuid()}");

        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetAllEnabledAsync_ReturnsOnlyEnabled()
    {
        NewsSource enabled1 = BuildSource($"enabled1-{Guid.NewGuid()}", enabled: true);
        NewsSource enabled2 = BuildSource($"enabled2-{Guid.NewGuid()}", enabled: true);
        NewsSource disabled = BuildSource($"disabled-{Guid.NewGuid()}", enabled: false);
        await InsertDirectly(enabled1);
        await InsertDirectly(enabled2);
        await InsertDirectly(disabled);

        NewsSource[] result = await _sut.GetAllEnabledAsync();

        string[] ids = result.Select(s => s.Id).ToArray();
        ids.Should().Contain(new[] { enabled1.Id, enabled2.Id });
        ids.Should().NotContain(disabled.Id);
    }

    private Task InsertDirectly(NewsSource source)
    {
        return ExecuteDbOperationAsync(db =>
            db.GetCollection<NewsSource>(CollectionName).InsertOneAsync(source));
    }

    private static NewsSource BuildSource(string id, bool enabled) => new()
    {
        Id = id,
        Name = $"Name {id}",
        PublisherName = $"Publisher {id}",
        PublisherLink = $"https://{id}.test",
        Url = $"https://{id}.test/rss",
        Type = SourceType.Feed,
        CronSchedule = "*/30 * * * * *",
        Enabled = enabled,
    };
}
