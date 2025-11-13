using System.Runtime.CompilerServices;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YoutubeAPIPOC;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", async () =>
{
    YouTubeService youTubeService = new YouTubeService(
        new BaseClientService.Initializer()
        {
            ApiKey = "Replace Here"
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
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();