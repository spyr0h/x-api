namespace XApi.Adapters.Mysql.Categories.Models;

public class Category
{
    public int ID { get; set; }
    public string? Value { get; set; }
    public int Count { get; set; }
    public int RecentCount { get; set; }
}
