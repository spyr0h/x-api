﻿using Dapper;
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
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(3);

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

            var query = @"
                SELECT 
                    c.ID, 
                    c.Value, 
                    COUNT(v.ID) AS Count,
                    COUNT(DISTINCT CASE
                        WHEN v.ModifiedDate >= DATE_SUB(NOW(), INTERVAL 1 DAY) THEN v.ID
                        ELSE NULL
                    END) AS RecentCount
                FROM 
                    Pornstars c
                LEFT JOIN 
                    PornstarVideo cv ON c.ID = cv.PornstarsID
                LEFT JOIN 
                    Videos v ON cv.VideosID = v.ID
                GROUP BY 
                    c.ID, c.Value
                ORDER BY 
                    c.Value ASC;";
            var pornstars = await dbConnection.QueryAsync<Models.Pornstar>(query);

            cachedPornstars = pornstars.Select(pornstar => pornstar.Adapt<Pornstar>()).ToList();

            _cache.Set(_cacheKey, cachedPornstars, _cacheDuration);
        }

        return cachedPornstars!;
    }

    public async Task<IList<Pornstar>> ProvidePornstarsForIds(int[] ids)
    {
        var pornstars = await ProvideAllPornstars();
        return ids
            .Select(id => pornstars.FirstOrDefault(pornstar => pornstar.ID == id))
            .Where(pornstar => pornstar != null)
            .Cast<Pornstar>()
            .ToList();
    }

    public async Task<Pornstar?> ProvidePornstarForValue(string value)
    {
        var loweredValue = value.ToLower();
        var pornstars = await ProvideAllPornstars();
        return pornstars.FirstOrDefault(pornstar => pornstar.Value == loweredValue);
    }

    public async Task<IList<Pornstar>> ProvidePornstarsForNonCompleteValue(string value)
    {
        var loweredValue = value.ToLower();
        var pornstars = await ProvideAllPornstars();
        return pornstars
            .Where(pornstar => pornstar.Value!.Contains(loweredValue))
            .ToList();
    }
}
