using NotificationService.DAL.Entities;
using NotificationService.Models.Webhooks;

namespace NotificationService.DAL.Mappers;

public static class WebhookMapper
{
    public static WebhookSubscriptionModel ToModel(WebhookSubscriptionEntity subscriptionEntity)
    {
        return new WebhookSubscriptionModel
        {
            Id = subscriptionEntity.Id.ToString(),
            Url = subscriptionEntity.Url,
            Action = subscriptionEntity.Action,
            CreationTime = subscriptionEntity.CreationTime,
            Enabled = subscriptionEntity.Enabled,
        };
    }
}
