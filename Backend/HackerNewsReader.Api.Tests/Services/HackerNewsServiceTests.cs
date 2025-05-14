using HackerNewsReader.Api.Models;
using HackerNewsReader.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace HackerNewsReader.Api.Tests.Services;

public class HackerNewsServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<HackerNewsService>> _loggerMock;
    private readonly IMemoryCache _memoryCache;

    public HackerNewsServiceTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object);
        _loggerMock = new Mock<ILogger<HackerNewsService>>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public async Task GetNewestStoriesAsync_ReturnsStories()
    { 
        var storyIds = new[] { 1, 2, 3 };
        var stories = new List<HackerNewsItem>
        {
            new() { Id = 1, Title = "Test Story 1", Url = "http://test1.com" },
            new() { Id = 2, Title = "Test Story 2", Url = "http://test2.com" },
            new() { Id = 3, Title = "Test Story 3", Url = "http://test3.com" }
        };

        SetupHttpHandler(storyIds, stories);

        var service = new HackerNewsService(_httpClient, _memoryCache, _loggerMock.Object);
         
        var result = await service.GetNewestStoriesAsync(3);
         
        Assert.Equal(3, result.Count());
        Assert.Contains(result, x => x.Title == "Test Story 1");
    }

    [Fact]
    public async Task GetNewestStoriesAsync_UsesCacheProperly()
    { 
        var storyIds = new[] { 1, 2, 3 };
        var stories = new List<HackerNewsItem>
    {
        new() { Id = 1, Title = "Test Story 1" },
        new() { Id = 2, Title = "Test Story 2" },
        new() { Id = 3, Title = "Test Story 3" }
    };

        SetupHttpHandler(storyIds, stories);

        var service = new HackerNewsService(_httpClient, _memoryCache, _loggerMock.Object);
         
        var result1 = await service.GetNewestStoriesAsync(3);
         
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.AbsoluteUri.Contains("newstories.json")),
            ItExpr.IsAny<CancellationToken>());

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(3), 
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.AbsoluteUri.Contains("item/")),
            ItExpr.IsAny<CancellationToken>());
         
        var result2 = await service.GetNewestStoriesAsync(3);
         
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1), 
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.AbsoluteUri.Contains("newstories.json")),
            ItExpr.IsAny<CancellationToken>());

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(3), 
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.AbsoluteUri.Contains("item/")),
            ItExpr.IsAny<CancellationToken>());


        _memoryCache.Remove("new_stories"); 
        var result3 = await service.GetNewestStoriesAsync(3);
         
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(2), 
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.AbsoluteUri.Contains("newstories.json")),
            ItExpr.IsAny<CancellationToken>());

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(3), 
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.AbsoluteUri.Contains("item/")),
            ItExpr.IsAny<CancellationToken>());
    }
     

    [Fact]
    public async Task SearchStoriesAsync_ReturnsMatchingStories()
    { 
        var storyIds = new[] { 1, 2, 3 };
        var stories = new List<HackerNewsItem>
        {
            new() { Id = 1, Title = "Angular is great", Url = "http://test1.com" },
            new() { Id = 2, Title = "React is cool", Url = "http://test2.com" },
            new() { Id = 3, Title = "Vue is nice", Url = "http://test3.com" }
        };

        SetupHttpHandler(storyIds, stories);

        var service = new HackerNewsService(_httpClient, _memoryCache, _loggerMock.Object);
         
        var result = await service.SearchStoriesAsync("angular", 10);
         
        Assert.Single(result);
        Assert.Equal("Angular is great", result.First().Title);
    }

    private void SetupHttpHandler(int[] storyIds, List<HackerNewsItem> stories)
    { 
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.AbsoluteUri.Contains("newstories.json")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(storyIds))
            });
         
        foreach (var story in stories)
        {
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.AbsoluteUri.Contains($"item/{story.Id}.json")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(story))
                });
        }
    }
}