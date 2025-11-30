using AutoFixture;
using FluentAssertions;
using Moq;
using NewsService.BLL.Abstractions.Validators;
using NewsService.DAL.Abstractions.Stores;
using NewsService.Models.News.Models;

namespace NewsService.BLL.Tests.Services;

public class NewsServiceTests
{
    private Mock<INewsStore> _newsStoreMock;
    private Mock<INewsValidator> _validatorMock;
    private BLL.Services.NewsService _sut;
    private Fixture _fixture = new Fixture();

    public NewsServiceTests()
    {
        _validatorMock = new Mock<INewsValidator>();
        _newsStoreMock = new Mock<INewsStore>();
        _sut = new BLL.Services.NewsService(_newsStoreMock.Object, _validatorMock.Object);
    }
    [Fact]
    public async Task GetManyAsync_FilterValid_Success()
    {
        var filter = _fixture.Create<NewsFilterModel>();

        Func<Task> act = () => _sut.GetManyAsync(filter);

        await act.Should().NotThrowAsync<Exception>();
        _newsStoreMock.Verify(x => x.GetManyAsync(It.Is(filter.IsEqualTo())), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidModel_Success()
    {
        var createModel = _fixture.Create<NewsCreateModel>();

        Func<Task> act = () => _sut.CreateAsync(createModel);

        await act.Should().NotThrowAsync<Exception>();
        _validatorMock.Verify(x => x.Validate(It.Is(createModel.IsEqualTo())), Times.Once);
        _newsStoreMock.Verify(x => x.CreateAsync(It.Is(createModel.IsEqualTo())), Times.Once);
    }

    [Fact]
    public async Task CreateBulkAsync_ValidModels_Success()
    {
        var models = _fixture.CreateMany<NewsCreateModel>(3).ToArray();

        Func<Task> act = () => _sut.CreateBulkAsync(models);

        await act.Should().NotThrowAsync<Exception>();
        _validatorMock.Verify(x => x.ValidateBulk(It.Is(models.IsEqualTo())), Times.Once);
        _newsStoreMock.Verify(x => x.CreateBulkAsync(It.Is(models.IsEqualTo())), Times.Once);
    }

}