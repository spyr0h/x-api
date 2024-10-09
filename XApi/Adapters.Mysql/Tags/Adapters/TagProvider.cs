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
    private readonly TimeSpan _cacheDuration = TimeSpan.FromDays(1);

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

            var query = "SELECT * FROM Tags";
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
}
