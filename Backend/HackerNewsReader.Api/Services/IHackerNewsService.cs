using HackerNewsReader.Api.Models;

namespace HackerNewsReader.Api.Services;

public interface IHackerNewsService
{
    Task<IEnumerable<HackerNewsItem>> GetNewestStoriesAsync(int count);
    Task<IEnumerable<HackerNewsItem>> SearchStoriesAsync(string query, int count);
}