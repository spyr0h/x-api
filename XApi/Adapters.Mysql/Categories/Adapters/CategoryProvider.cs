using Dapper;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using System.Data;
using XApi.Core.Categories.Models;
using XApi.Core.Categories.Ports.Interfaces;

namespace XApi.Adapters.Mysql.Categories.Adapters;

public class CategoryProvider : ICategoryProvider
{
    private readonly string _connectionString;
    private readonly IMemoryCache _cache;
    private readonly string _cacheKey = "AllCategories";
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(3);

    private IDbConnection Connection => new MySqlConnection(_connectionString);

    public CategoryProvider(IMemoryCache cache)
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
        _cache = cache;
    }

    public async Task<IList<Category>> ProvideAllCategories()
    {
        if (!_cache.TryGetValue(_cacheKey, out IList<Category>? cachedCategories))
        {
            using var dbConnection = Connection;
            dbConnection.Open();

            var query = @"
                SELECT 
                    c.ID, 
                    c.Value,
                    c.Title,
                    c.Description,
                    COUNT(v.ID) AS Count,
                    COUNT(DISTINCT CASE
                        WHEN v.ModifiedDate >= DATE_SUB(NOW(), INTERVAL 1 DAY) THEN v.ID
                        ELSE NULL
                    END) AS RecentCount
                FROM 
                    Categories c
                LEFT JOIN 
                    CategoryVideo cv ON c.ID = cv.CategoriesID
                LEFT JOIN 
                    Videos v ON cv.VideosID = v.ID
                GROUP BY 
                    c.ID, c.Value
                ORDER BY 
                    c.Value ASC;";
            var categories = await dbConnection.QueryAsync<Models.Category>(query);

            cachedCategories = categories.Select(category => category.Adapt<Category>()).ToList();

            _cache.Set(_cacheKey, cachedCategories, _cacheDuration);
        }

        return cachedCategories!;
    }

    public async Task<IList<Category>> ProvideCategoriesForIds(int[] ids)
    {
        var categories = await ProvideAllCategories();
        return ids
            .Select(id => categories.FirstOrDefault(category => category.ID == id))
            .Where(category => category != null)
            .Cast<Category>()
            .ToList();
    }

    public async Task<Category?> ProvideCategoryForValue(string value)
    {
        var loweredValue = value.ToLower();
        var categories = await ProvideAllCategories();
        return categories.FirstOrDefault(category => category.Value! == loweredValue);
    }

    public async Task<IList<Category>> ProvideCategoriesForNonCompleteValue(string value)
    {
        var loweredValue = value.ToLower();
        var categories = await ProvideAllCategories();
        return categories
            .Where(category => category.Value!.Contains(loweredValue))
            .ToList();
    }
}
