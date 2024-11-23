namespace XApi.Adapters.Mysql.Tags.Models;

public class Tag
{
    public int ID { get; set; }
    public string? Value { get; set; }
    public int Count { get; set; }
    public int RecentCount { get; set; }
}
