namespace XApi.Adapters.Mysql.Videos.Models;

public class Video
{
    public int VideoID { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public int? Year { get; set; }
    public List<VideoTag> Tags { get; set; } = [];
    public List<VideoPornstar> Pornstars { get; set; } = [];
}
