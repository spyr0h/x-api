using Dapper;
using Mapster;
using System.Data;
using XApi.Core.Tags.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Caching.Memory;
using XApi.Core.Tags.Ports.Interfaces;

namespace XApi.Adapters.Mysql.Tags.Adapters;

public class TagProvider : ITagProvider
{
    private readonly string _connectionString;
    private readonly IMemoryCache _cache;
    private readonly string _cacheKey = "AllTags";
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(3);

    private IDbConnection Connection => new MySqlConnection(_connectionString);

    public TagProvider(IMemoryCache cache)
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
        _cache = cache;
    }

    public async Task<IList<Tag>> ProvideAllTags()
    {
        if (!_cache.TryGetValue(_cacheKey, out IList<Tag>? cachedTags))
        {
            using var dbConnection = Connection;
            dbConnection.Open();

            var query = @"
                SELECT 
                    c.ID, 
                    c.Value, 
                    COUNT(v.ID) AS Count,
                    SUM(CASE 
                        WHEN v.ModifiedDate >= DATE_SUB(NOW(), INTERVAL 1 DAY) THEN 1 
                        ELSE 0 
                    END) AS RecentCount
                FROM 
                    Tags c
                LEFT JOIN 
                    TagVideo cv ON c.ID = cv.TagsID
                LEFT JOIN 
                    Videos v ON cv.VideosID = v.ID
                GROUP BY 
                    c.ID, c.Value
                ORDER BY 
                    c.Value ASC;";
            var tags = await dbConnection.QueryAsync<Models.Tag>(query);

            cachedTags = tags.Select(tag => tag.Adapt<Tag>()).ToList();

            _cache.Set(_cacheKey, cachedTags, _cacheDuration);
        }

        return cachedTags!;
    }

    public async Task<IList<Tag>> ProvideTagsForIds(int[] ids)
    {
        var tags = await ProvideAllTags();
        return ids
            .Select(id => tags.FirstOrDefault(tag => tag.ID == id))
            .Where(tag => tag != null)
            .Cast<Tag>()
            .ToList();
    }

    public async Task<Tag?> ProvideTagForValue(string value)
    {
        var loweredValue = value.ToLower();
        var tags = await ProvideAllTags();
        return tags.FirstOrDefault(tag => tag.Value == loweredValue);
    }

    public async Task<IList<Tag>> ProvideTagsForNonCompleteValue(string value)
    {
        var loweredValue = value.ToLower();
        var tags = await ProvideAllTags();
        return tags
            .Where(tag => tag.Value!.Contains(loweredValue))
            .ToList();
    }
}
