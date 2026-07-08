using Common.Enums;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using NotificationService.BLL.Abstractions.Services;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/v1/webhooks")]
public class WebhooksControllers : ControllerBase
{
    private readonly IWebhookSubscriptionsServices _service;

    public WebhooksControllers(IWebhookSubscriptionsServices service)
    {
        _service = service;
    }

    [HttpGet("")]
    public async Task<List<WebhookSubscriptionModel>> GetMany([FromQuery] WebhookFilterRequest request)
    {
        WebhooksFilter filter = new WebhooksFilter
        {
            Actions = request.Actions,
            Enabled = request.Enabled,
        };
        OffsetPagination pagination = new OffsetPagination(
            Math.Max(0, request.Offset),
            request.Take > 0 ? request.Take : 100);

        OffsetCollection<WebhookSubscriptionModel> result =
            await _service.GetManyAsync(filter, pagination);

        this.AddPaginationHeaders(result);
        return result.ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WebhookSubscriptionModel>> GetById(string id)
    {
        WebhookSubscriptionModel? model = await _service.GetByIdAsync(id);
        return model is null ? NotFound() : Ok(model);
    }

    [HttpPost("")]
    public async Task<ActionResult<string>> Subscribe([FromBody] WebhookCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url) || string.IsNullOrWhiteSpace(request.Event))
        {
            return BadRequest("Url and Event are required.");
        }

        string id = await _service.AddAsync(request.Url, request.Event);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] WebhookUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url) || string.IsNullOrWhiteSpace(request.Event))
        {
            return BadRequest("Url and Event are required.");
        }

        bool updated = await _service.UpdateAsync(id, request.Url, request.Event, request.Enabled);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        bool deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

public class WebhookCreateRequest
{
    public string Url { get; set; } = null!;
    public string Event { get; set; } = null!;
}

public class WebhookUpdateRequest
{
    public string Url { get; set; } = null!;
    public string Event { get; set; } = null!;
    public bool Enabled { get; set; } = true;
}

public class WebhookFilterRequest
{
    public string[]? Actions { get; set; }
    public bool? Enabled { get; set; }
    public int Offset { get; set; }
    public int Take { get; set; } = 100;
    public OrderDirection OrderDirection { get; set; } = OrderDirection.Descending;
}
