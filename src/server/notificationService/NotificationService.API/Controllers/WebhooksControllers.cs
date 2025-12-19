using Microsoft.AspNetCore.Mvc;
using NotificationService.BLL.Abstractions.Services;

namespace NotificationService.Controllers;

public class WebhooksControllers: ControllerBase
{
    private readonly IWebhooksServices _service;

    public WebhooksControllers(IWebhooksServices service)
    {
        _service = service;
    }

    [HttpPost("webhooks")]
    public async Task AddWebhook([FromBody] WebhookCreateRequest request)
    {
        await _service.AddAsync(request.Url, request.Action);
    }
    
}

public class WebhookCreateRequest
{
    public string Url { get; set; }
    public string Action { get; set; }
}