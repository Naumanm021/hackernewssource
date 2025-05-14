using HackerNewsReader.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace HackerNewsReader.Api.Services;

public class HackerNewsService : IHackerNewsService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<HackerNewsService> _logger;

    private const string NewStoriesCacheKey = "new_stories";
    private const string StoryCachePrefix = "story_";
    private const string BaseUrl = "https://hacker-news.firebaseio.com/v0/";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public HackerNewsService(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<HackerNewsService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<HackerNewsItem>> GetNewestStoriesAsync(int count)
    {
        try
        { 
            var storyIds = await _cache.GetOrCreateAsync(NewStoriesCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                var response = await _httpClient.GetAsync($"{BaseUrl}newstories.json");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<int[]>(content) ?? Array.Empty<int>();
            });
             
            var stories = new List<HackerNewsItem>();
            foreach (var id in storyIds?.Take(count) ?? Array.Empty<int>())
            {
                var story = await GetStoryWithCache(id);
                if (story != null && !string.IsNullOrEmpty(story.Title))
                {
                    stories.Add(story);
                }
            }

            return stories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching newest stories");
            throw;
        }
    }

    private async Task<HackerNewsItem?> GetStoryWithCache(int id)
    {
        return await _cache.GetOrCreateAsync($"{StoryCachePrefix}{id}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}item/{id}.json");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<HackerNewsItem>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching story {id}");
                return null;
            }
        });
    }

    public async Task<IEnumerable<HackerNewsItem>> SearchStoriesAsync(string query, int count)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<HackerNewsItem>();

        var stories = await GetNewestStoriesAsync(500); 
        return stories
            .Where(s => s.Title != null &&
                       s.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(count);
    }
}