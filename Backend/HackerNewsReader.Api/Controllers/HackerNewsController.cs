using HackerNewsReader.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HackerNewsController : ControllerBase
{
    private readonly IHackerNewsService _hackerNewsService;
    private readonly ILogger<HackerNewsController> _logger;

    public HackerNewsController(
        IHackerNewsService hackerNewsService,
        ILogger<HackerNewsController> logger)
    {
        _hackerNewsService = hackerNewsService;
        _logger = logger;
    }

    [HttpGet("newest")]
    public async Task<IActionResult> GetNewestStories([FromQuery] int count = 10)
    {
        try
        {
            var stories = await _hackerNewsService.GetNewestStoriesAsync(count);
            return Ok(stories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting newest stories");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchStories([FromQuery] string query, [FromQuery] int count = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var stories = await _hackerNewsService.SearchStoriesAsync(query, count);
            return Ok(stories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching stories");
            return StatusCode(500, "Internal server error");
        }
    }
}