using Core.Tags.Ports.Interfaces;
using Dapper;
using Mapster;
using System.Data;
using XApi.Core.Tags.Models;
using MySql.Data.MySqlClient;

namespace XApi.Adapters.Mysql.Tags.Adapters;

public class TagProvider : ITagProvider
{
    private readonly string _connectionString;

    public TagProvider()
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
    }

    private IDbConnection Connection => new MySqlConnection(_connectionString);

    public async Task<IList<Tag>> ProvideAllTags()
    {
        using var dbConnection = Connection;
        dbConnection.Open();

        var query = "SELECT * FROM Tags";
        
        var tags = await dbConnection.QueryAsync<Models.Tag>(query);

        return tags
            .Select(tag => tag.Adapt<Tag>())
            .ToList();
    }
}
