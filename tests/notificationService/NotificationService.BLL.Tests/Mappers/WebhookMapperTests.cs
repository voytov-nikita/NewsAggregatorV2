using AutoFixture;
using FluentAssertions;
using MongoDB.Bson;
using NotificationService.DAL.Entities;
using NotificationService.DAL.Mappers;

namespace NotificationService.BLL.Tests.Mappers;

public class WebhookMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToModel_WebhookSubscriptionEntity_ReturnsWebhookSubscriptionModel()
    {
        var entity = new WebhookSubscriptionEntity
        {
            Id = ObjectId.GenerateNewId(),
            Url = _fixture.Create<string>(),
            Action = _fixture.Create<string>(),
            CreationTime = DateTime.UtcNow
        };

        var result = WebhookMapper.ToModel(entity);

        result.Id.Should().Be(entity.Id.ToString());
        result.Url.Should().Be(entity.Url);
        result.Action.Should().Be(entity.Action);
        result.CreationTime.Should().Be(entity.CreationTime);
    }
}
