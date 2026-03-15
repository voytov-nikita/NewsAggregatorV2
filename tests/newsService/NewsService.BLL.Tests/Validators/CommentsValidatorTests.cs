using AutoFixture;
using FluentAssertions;
using NewsService.BLL.Validators;
using NewsService.Models.Comments.Models;

namespace NewsService.BLL.Tests.Validators;

public class CommentsValidatorTests
{
    private readonly CommentsValidator _sut = new CommentsValidator();
    private readonly Fixture _fixture = new Fixture();

    #region Validate(CommentCreateModel)

    [Fact]
    public void Validate_CommentCreateModelValid_Success()
    {
        var model = _fixture.Create<CommentCreateModel>();

        Action act = () => _sut.Validate(model);

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_CommentCreateModelEmptyContent_ThrowsArgumentException()
    {
        var model = _fixture.Create<CommentCreateModel>();
        model.Content = "";

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("Content is empty");
    }

    [Fact]
    public void Validate_CommentCreateModelNullContent_ThrowsArgumentException()
    {
        var model = _fixture.Create<CommentCreateModel>();
        model.Content = null;

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("Content is empty");
    }

    [Fact]
    public void Validate_CommentCreateModelContentTooLong_ThrowsArgumentException()
    {
        var model = _fixture.Create<CommentCreateModel>();
        model.Content = new string('a', 1001);

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Validate(CommentUpdateModel)

    [Fact]
    public void Validate_CommentUpdateModelValid_Success()
    {
        var model = _fixture.Create<CommentUpdateModel>();

        Action act = () => _sut.Validate(model);

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_CommentUpdateModelEmptyContent_ThrowsArgumentException()
    {
        var model = _fixture.Create<CommentUpdateModel>();
        model.Content = "";

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("Content is empty");
    }

    [Fact]
    public void Validate_CommentUpdateModelNullContent_ThrowsArgumentException()
    {
        var model = _fixture.Create<CommentUpdateModel>();
        model.Content = null;

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>().WithMessage("Content is empty");
    }

    [Fact]
    public void Validate_CommentUpdateModelContentTooLong_ThrowsArgumentException()
    {
        var model = _fixture.Create<CommentUpdateModel>();
        model.Content = new string('a', 1001);

        Action act = () => _sut.Validate(model);

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Validate(string rateType)

    [Theory]
    [InlineData("Like")]
    [InlineData("Dislike")]
    public void Validate_ValidRateType_Success(string rateType)
    {
        Action act = () => _sut.Validate(rateType);

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_EmptyRateType_ThrowsArgumentException()
    {
        Action act = () => _sut.Validate("");

        act.Should().Throw<ArgumentException>().WithMessage("RateType is empty");
    }

    [Fact]
    public void Validate_NullRateType_ThrowsArgumentException()
    {
        Action act = () => _sut.Validate((string)null);

        act.Should().Throw<ArgumentException>().WithMessage("RateType is empty");
    }

    [Fact]
    public void Validate_InvalidRateType_ThrowsArgumentException()
    {
        Action act = () => _sut.Validate("InvalidType");

        act.Should().Throw<ArgumentException>().WithMessage("RateType is not valid");
    }

    #endregion
}
