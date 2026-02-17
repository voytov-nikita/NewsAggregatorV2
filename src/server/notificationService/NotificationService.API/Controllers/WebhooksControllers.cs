using Microsoft.AspNetCore.Mvc;
using NotificationService.BLL.Abstractions.Services;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/v1/webhooks")]
public class WebhooksControllers: ControllerBase
{
    private readonly IWebhookSubscriptionsServices _service;

    public WebhooksControllers(IWebhookSubscriptionsServices service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task Subscribe([FromBody] WebhookCreateRequest request)
    {
        await _service.AddAsync(request.Url, request.Event);
    }
    
}

public class WebhookCreateRequest
{
    public string Url { get; set; }
    public string Event { get; set; }
}