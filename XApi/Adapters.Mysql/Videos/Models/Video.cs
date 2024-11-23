namespace XApi.Adapters.Mysql.Videos.Models;

public class Video
{
    public int VideoID { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? Year { get; set; }
    public string Tags { get; set; }
    public string Categories { get; set; }
    public string Pornstars { get; set; } 
    public string Links { get; set; }
    public string Pictures { get; set; }
}
