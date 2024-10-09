using Dapper;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using System.Data;
using XApi.Core.Pornstars.Models;
using XApi.Core.Pornstars.Ports.Interfaces;

namespace XApi.Adapters.Mysql.Pornstars.Adapters;

public class PornstarProvider : IPornstarProvider
{
    private readonly string _connectionString;
    private readonly IMemoryCache _cache;
    private readonly string _cacheKey = "AllPornstars";
    private readonly TimeSpan _cacheDuration = TimeSpan.FromDays(1);

    private IDbConnection Connection => new MySqlConnection(_connectionString);

    public PornstarProvider(IMemoryCache cache)
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
        _cache = cache;
    }

    public async Task<IList<Pornstar>> ProvideAllPornstars()
    {
        if (!_cache.TryGetValue(_cacheKey, out IList<Pornstar>? cachedPornstars))
        {
            using var dbConnection = Connection;
            dbConnection.Open();

            var query = "SELECT * FROM Pornstars";
            var pornstars = await dbConnection.QueryAsync<Models.Pornstar>(query);

            cachedPornstars = pornstars.Select(pornstar => pornstar.Adapt<Pornstar>()).ToList();

            _cache.Set(_cacheKey, cachedPornstars, _cacheDuration);
        }

        return cachedPornstars!;
    }

    public async Task<IList<Pornstar>> ProvidePornstarsForIds(int[] ids)
    {
        var tags = await ProvideAllPornstars();
        return ids
            .Select(id => tags.FirstOrDefault(tag => tag.ID == id))
            .Where(tag => tag != null)
            .Cast<Pornstar>()
            .ToList();
    }
}
