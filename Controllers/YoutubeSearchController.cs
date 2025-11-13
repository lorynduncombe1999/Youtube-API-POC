using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using YoutubeAPIPOC.Configurations;
using ILogger = Google.Apis.Logging.ILogger;

namespace YoutubeAPIPOC.Controllers;
[ApiController]
[Route("[controller]")]
public class YoutubeSearchController:ControllerBase
{
    private readonly ILogger<YoutubeSearchController> _logger;
    private readonly SearchOptions _searchOptions;

    public YoutubeSearchController(ILogger<YoutubeSearchController> logger,IOptions<SearchOptions> searchOptions)
    {
        _logger = logger;
        _searchOptions = searchOptions.Value;
    }
    
    [HttpGet(Name = "YoutubeSearch")]
    public async Task<List<string>> Get(string searchRequest)
    {
        //Replace Typed in information with Ioptions pattern so that
        YouTubeService youTubeService = new YouTubeService(
            new BaseClientService.Initializer()
            {

                ApiKey = _searchOptions.ApiKey
            });
        var searchListRequest = youTubeService.Search.List(_searchOptions.Part);
        searchListRequest.Q = searchRequest; // Replace with your search term.
        searchListRequest.MaxResults = _searchOptions.MaxResult;

        var searchListResponse = await searchListRequest.ExecuteAsync();
        var result = ResultParser(searchListResponse);
       
        return result;
    }

    [NonAction]
    private List<string> ResultParser(SearchListResponse response)
    {
        List<string> videos = new List<string>();
        List<string> channels = new List<string>();
        List<string> playlists = new List<string>();
        foreach (var searchResult in response.Items)
        {
            switch (searchResult.Id.Kind)
            {
                case "youtube#video":
                    videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                    break;

                case "youtube#channel":
                    channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                    break;

                case "youtube#playlist":
                    playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                    break;
            }
        }
        return videos;
    }
}