namespace XApi.Adapters.Mysql.Videos.Models;

public class Stat
{
    public int ID { get; set; }
    public int Clicks { get; set; }
    public int VideoId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
