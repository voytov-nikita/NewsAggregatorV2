using System;
using AutoFixture;
using FluentAssertions;
using NewsService.BLL.Validators;
using NewsService.Models.News.Models;
using Xunit;

namespace NewsService.BLL.Tests.Validators;

public class NewsValidatorTests
{
    private readonly NewsValidator _sut = new NewsValidator();
    private readonly Fixture _fixture = new Fixture();

    [Fact]
    public void Validate_ModelValid_Success()
    {
        var model = _fixture.Create<NewsCreateModel>();

        Action act = () => _sut.Validate(model);

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_MissingTitle_ThrowsArgumentException()
    {
        var model = _fixture.Create<NewsCreateModel>();
        model.Title = null;

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("Title cannot be empty");
    }

    [Fact]
    public void Validate_MissingOriginalLink_ThrowsArgumentException()
    {
        var model = _fixture.Create<NewsCreateModel>();
        model.OriginalLink = "";

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("OriginalLink cannot be empty");
    }

    [Fact]
    public void Validate_MissingPublisher_ThrowsArgumentException()
    {
        var model = _fixture.Create<NewsCreateModel>();
        model.Publisher = null;

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("Publisher cannot be empty");
    }

    [Fact]
    public void Validate_MissingPublisherLink_ThrowsArgumentException()
    {
        var model = _fixture.Create<NewsCreateModel>();
        model.PublisherLink = "";

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("PublisherLink cannot be empty");
    }

    [Fact]
    public void Validate_MissingGuid_ThrowsArgumentException()
    {
        var model = _fixture.Create<NewsCreateModel>();
        model.Guid = null;

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("Guid cannot be empty");
    }

    [Fact]
    public void ValidateBulk_AllValid_Success()
    {
        var models = _fixture.CreateMany<NewsCreateModel>(3).ToArray();

        Action act = () => _sut.ValidateBulk(models);

        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateBulk_ContainsInvalid_ThrowsArgumentException()
    {
        var models = _fixture.CreateMany<NewsCreateModel>(2).ToArray();
        models[1].Publisher = "";

        Action act = () => _sut.ValidateBulk(models);

        act.Should().Throw<ArgumentException>().WithMessage("Publisher cannot be empty");
    }
}
