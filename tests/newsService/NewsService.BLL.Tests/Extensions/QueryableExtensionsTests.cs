using Common.Enums;
using FluentAssertions;
using NewsService.DAL.PostgreSql.Extensions;

namespace NewsService.BLL.Tests.Extensions;

public class QueryableExtensionsTests
{
    private readonly List<TestItem> _items =
    [
        new() { Id = 3, Name = "Charlie" },
        new() { Id = 1, Name = "Alice" },
        new() { Id = 2, Name = "Bob" },
    ];

    #region OrderBy IEnumerable

    [Fact]
    public void OrderBy_Enumerable_Ascending_ReturnsAscendingOrder()
    {
        var result = _items.OrderBy(x => x.Id, OrderDirection.Ascending).ToList();

        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);
        result[2].Id.Should().Be(3);
    }

    [Fact]
    public void OrderBy_Enumerable_Descending_ReturnsDescendingOrder()
    {
        var result = _items.OrderBy(x => x.Id, OrderDirection.Descending).ToList();

        result[0].Id.Should().Be(3);
        result[1].Id.Should().Be(2);
        result[2].Id.Should().Be(1);
    }

    #endregion

    #region ThenBy IEnumerable

    [Fact]
    public void ThenBy_Enumerable_Ascending_ReturnsSortedBySecondaryKey()
    {
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "Bob" },
            new() { Id = 1, Name = "Alice" },
            new() { Id = 2, Name = "Charlie" },
        };

        var result = items
            .OrderBy(x => x.Id, OrderDirection.Ascending)
            .ThenBy(x => x.Name, OrderDirection.Ascending)
            .ToList();

        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
    }

    [Fact]
    public void ThenBy_Enumerable_Descending_ReturnsSortedBySecondaryKeyDescending()
    {
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "Alice" },
            new() { Id = 1, Name = "Bob" },
            new() { Id = 2, Name = "Charlie" },
        };

        var result = items
            .OrderBy(x => x.Id, OrderDirection.Ascending)
            .ThenBy(x => x.Name, OrderDirection.Descending)
            .ToList();

        result[0].Name.Should().Be("Bob");
        result[1].Name.Should().Be("Alice");
    }

    #endregion

    #region OrderBy IQueryable

    [Fact]
    public void OrderBy_Queryable_Ascending_ReturnsAscendingOrder()
    {
        var result = _items.AsQueryable().OrderBy(x => x.Id, OrderDirection.Ascending).ToList();

        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);
        result[2].Id.Should().Be(3);
    }

    [Fact]
    public void OrderBy_Queryable_Descending_ReturnsDescendingOrder()
    {
        var result = _items.AsQueryable().OrderBy(x => x.Id, OrderDirection.Descending).ToList();

        result[0].Id.Should().Be(3);
        result[1].Id.Should().Be(2);
        result[2].Id.Should().Be(1);
    }

    #endregion

    #region ThenBy IQueryable

    [Fact]
    public void ThenBy_Queryable_Ascending_ReturnsSortedBySecondaryKey()
    {
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "Bob" },
            new() { Id = 1, Name = "Alice" },
            new() { Id = 2, Name = "Charlie" },
        };

        var result = items.AsQueryable()
            .OrderBy(x => x.Id, OrderDirection.Ascending)
            .ThenBy(x => x.Name, OrderDirection.Ascending)
            .ToList();

        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
    }

    [Fact]
    public void ThenBy_Queryable_Descending_ReturnsSortedBySecondaryKeyDescending()
    {
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "Alice" },
            new() { Id = 1, Name = "Bob" },
            new() { Id = 2, Name = "Charlie" },
        };

        var result = items.AsQueryable()
            .OrderBy(x => x.Id, OrderDirection.Ascending)
            .ThenBy(x => x.Name, OrderDirection.Descending)
            .ToList();

        result[0].Name.Should().Be("Bob");
        result[1].Name.Should().Be("Alice");
    }

    #endregion

    private class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
