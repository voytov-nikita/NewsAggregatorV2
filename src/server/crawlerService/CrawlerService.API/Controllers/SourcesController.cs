using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrawlerService.API.Controllers;

/// <summary>
/// CRUD for <see cref="NewsSource"/> stored in Mongo. Every mutation goes
/// through <see cref="ISourceSchedulingService"/> so Hangfire recurring jobs
/// stay in sync with the collection.
/// </summary>
[ApiController]
[Route("api/v1/sources")]
public class SourcesController : ControllerBase
{
    private readonly ISourceStore _store;
    private readonly ISourceSchedulingService _scheduling;

    public SourcesController(ISourceStore store, ISourceSchedulingService scheduling)
    {
        _store = store;
        _scheduling = scheduling;
    }

    [HttpGet("")]
    public async Task<NewsSource[]> GetAll() =>
        await _store.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<NewsSource>> GetById(string id)
    {
        NewsSource? source = await _store.GetByIdAsync(id);
        return source is null ? NotFound() : Ok(source);
    }

    [HttpPost("")]
    public async Task<ActionResult<NewsSource>> Create([FromBody] NewsSource source)
    {
        if (string.IsNullOrWhiteSpace(source.Id))
        {
            return BadRequest("Id is required.");
        }
        if (!IsValidCron(source.CronSchedule))
        {
            return BadRequest("CronSchedule is invalid.");
        }

        NewsSource? existing = await _store.GetByIdAsync(source.Id);
        if (existing is not null)
        {
            return Conflict($"Source with id '{source.Id}' already exists.");
        }

        await _store.UpsertAsync(source);
        _scheduling.Schedule(source);

        return CreatedAtAction(nameof(GetById), new { id = source.Id }, source);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NewsSource>> Update(string id, [FromBody] NewsSource source)
    {
        if (!string.Equals(id, source.Id, StringComparison.Ordinal))
        {
            return BadRequest("Route id does not match body id.");
        }
        if (!IsValidCron(source.CronSchedule))
        {
            return BadRequest("CronSchedule is invalid.");
        }

        NewsSource? existing = await _store.GetByIdAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        await _store.UpsertAsync(source);
        _scheduling.Schedule(source);

        return Ok(source);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        bool deleted = await _store.DeleteAsync(id);
        if (!deleted) return NotFound();

        _scheduling.Unschedule(id);
        return NoContent();
    }

    [HttpPut("{id}/toggle")]
    public async Task<ActionResult<NewsSource>> Toggle(string id)
    {
        NewsSource? source = await _store.GetByIdAsync(id);
        if (source is null) return NotFound();

        source.Enabled = !source.Enabled;
        await _store.UpsertAsync(source);
        _scheduling.Schedule(source);

        return Ok(source);
    }

    /// <summary>
    /// Cheap syntactic sanity check — not full cron validation. Hangfire will
    /// throw on schedule if the expression is malformed, which returns 500;
    /// this catches the common "empty" / "wrong field count" cases early.
    /// </summary>
    private static bool IsValidCron(string cron)
    {
        if (string.IsNullOrWhiteSpace(cron)) return false;
        int parts = cron.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return parts is 5 or 6;
    }
}
