namespace YoutubeAPIPOC.Configurations;

public class SearchOptions
{
    public string Part = "snippet";
    public string ApiKey { get; set; }
    public int MaxResult { get; set; }

}