using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using ILogger = Google.Apis.Logging.ILogger;

namespace YoutubeAPIPOC.Controllers;
[ApiController]
[Route("[controller]")]
public class YoutubeSearchController:ControllerBase
{
    private readonly ILogger<YoutubeSearchController> _logger;

    public YoutubeSearchController(ILogger<YoutubeSearchController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet(Name = "YoutubeSearch")]
    public async Task<List<string>> Get()
    {
        //Replace Typed in information with Ioptions pattern so that
        YouTubeService youTubeService = new YouTubeService(
            new BaseClientService.Initializer()
            {
                ApiKey = "ReplaceMe"
            });
        var searchListRequest = youTubeService.Search.List("snippet");
        searchListRequest.Q = "Google"; // Replace with your search term.
        searchListRequest.MaxResults = 50;

        var searchListResponse = await searchListRequest.ExecuteAsync();
    
        List<string> videos = new List<string>();
        List<string> channels = new List<string>();
        List<string> playlists = new List<string>();
    
        // Add each result to the appropriate list, and then display the lists of
        // matching videos, channels, and playlists.
        foreach (var searchResult in searchListResponse.Items)
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