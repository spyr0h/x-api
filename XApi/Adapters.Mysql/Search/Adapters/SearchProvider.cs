﻿using Mapster;
using MySql.Data.MySqlClient;
using System.Data;
using XApi.Core.Host.Enums;
using XApi.Core.Pictures.Models;
using XApi.Core.Search.Models;
using XApi.Core.Search.Ports.Interfaces;

namespace XApi.Adapters.Mysql.Search.Adapters;

public class SearchProvider : ISearchProvider
{
    private readonly string _connectionString;
    private readonly string _query = @"
        SET SESSION group_concat_max_len = 1000000;

        SELECT 
            v.ID AS VideoID,
            v.Title,
            v.Description,
            v.Duration,
            v.Year,
            v.CreatedDate,
            v.ModifiedDate,
            GROUP_CONCAT(DISTINCT CONCAT(t.ID, 'µ', IFNULL(t.Value, 'NULL')) SEPARATOR '|') AS Tags,
            GROUP_CONCAT(DISTINCT CONCAT(c.ID, 'µ', IFNULL(c.Value, 'NULL')) SEPARATOR '|') AS Categories,
            GROUP_CONCAT(DISTINCT CONCAT(p.ID, 'µ', IFNULL(p.Value, 'NULL')) SEPARATOR '|') AS Pornstars,
            GROUP_CONCAT(DISTINCT CONCAT(hl.Url, 'µ', IFNULL(hl.Size, 'NULL'), 'µ', IFNULL(hl.Host, 'NULL'), 'µ', IFNULL(hl.Resolution, 'NULL'), 'µ', IFNULL(hl.Format, 'NULL'), 'µ', IFNULL(hl.Part, 'NULL')) SEPARATOR '|') AS Links,
            GROUP_CONCAT(DISTINCT CONCAT(pc.DirectUrl, 'µ', IFNULL(pc.HostUrl, 'NULL')) SEPARATOR '|') AS Pictures
        FROM
            Videos v
        LEFT JOIN
            TagVideo tv ON v.ID = tv.VideosID
        LEFT JOIN
            Tags t ON tv.TagsID = t.ID
        LEFT JOIN
            CategoryVideo cv ON v.ID = cv.VideosID
        LEFT JOIN
            Categories c on cv.CategoriesID = c.ID
        LEFT JOIN
            PornstarVideo pv ON v.ID = pv.VideosID
        LEFT JOIN
            Pornstars p ON pv.PornstarsID = p.ID
        LEFT JOIN
            HostLinks hl ON v.ID = hl.VideoId
        LEFT JOIN
            Pictures pc ON v.ID = pc.VideoId
        WHERE 
            (
                {0}
                v.ID IN (
                    SELECT VideosID 
                    FROM TagVideo 
                    WHERE TagsID IN {1}
                    GROUP BY VideosID 
                    HAVING COUNT(DISTINCT TagsID) = {2}
                )
            )
        AND 
            (
                {3}
                v.ID IN (
                    SELECT VideosID 
                    FROM PornstarVideo 
                    WHERE PornstarsID IN {4}
                    GROUP BY VideosID 
                    HAVING COUNT(DISTINCT PornstarsID) = {5}
                )
            )
        AND 
            (
                {6}
                v.ID IN (
                    SELECT VideosID 
                    FROM CategoryVideo 
                    WHERE CategoriesID IN {7}
                    GROUP BY VideosID 
                    HAVING COUNT(DISTINCT CategoriesID) = {8}
                )
            )
        GROUP BY 
            v.ID, v.Title, v.Description, v.Duration, v.Year, v.CreatedDate, v.ModifiedDate
        ORDER BY
	        v.ModifiedDate {9}
        LIMIT {10} OFFSET {11}
    ";

    private readonly string _countQuery = @"
        SELECT 
            COUNT(DISTINCT v.ID) as COUNT
        FROM
            Videos v
        LEFT JOIN
            TagVideo tv ON v.ID = tv.VideosID
        LEFT JOIN
            Tags t ON tv.TagsID = t.ID
        LEFT JOIN
            CategoryVideo cv ON v.ID = cv.VideosID
        LEFT JOIN
            Categories c on cv.CategoriesID = c.ID
        LEFT JOIN
            PornstarVideo pv ON v.ID = pv.VideosID
        LEFT JOIN
            Pornstars p ON pv.PornstarsID = p.ID
        LEFT JOIN
            HostLinks hl ON v.ID = hl.VideoId
        LEFT JOIN
            Pictures pc ON v.ID = pc.VideoId
        WHERE 
            (
                {0}
                v.ID IN (
                    SELECT VideosID 
                    FROM TagVideo 
                    WHERE TagsID IN {1}
                    GROUP BY VideosID 
                    HAVING COUNT(DISTINCT TagsID) = {2}
                )
            )
        AND 
            (
                {3}
                v.ID IN (
                    SELECT VideosID 
                    FROM PornstarVideo 
                    WHERE PornstarsID IN {4}
                    GROUP BY VideosID 
                    HAVING COUNT(DISTINCT PornstarsID) = {5}
                )
            )
        AND 
            (
                {6}
                v.ID IN (
                    SELECT VideosID 
                    FROM CategoryVideo 
                    WHERE CategoriesID IN {7}
                    GROUP BY VideosID 
                    HAVING COUNT(DISTINCT CategoriesID) = {8}
                )
            )
    ";

    private readonly string _defaultOrder = "DESC";
            
    private MySqlConnection Connection => new MySqlConnection(_connectionString);

    public SearchProvider()
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
    }

    public async Task<SearchResult> SearchVideosByCriteria(SearchCriteria searchCriteria)
    {
        if (searchCriteria == null) throw new Exception("The searchCriteria should not be null.");

        using MySqlConnection dbConnection = Connection;
        dbConnection.Open();

        var tags = (searchCriteria.Tags?.Any() ?? false) ? searchCriteria.Tags.Select(t => t.ID) : [-1];
        var tagsFalse = (searchCriteria.Tags ?? []).Any() ? "" : "1=1 OR ";
        var tagsIDs = $"({string.Join(",", tags)})";
        var tagsCount = searchCriteria.Tags?.Count() ?? 0;

        var categories = (searchCriteria.Categories?.Any() ?? false) ? searchCriteria.Categories.Select(t => t.ID) : [-1];
        var categoriesFalse = (searchCriteria.Categories ?? []).Any() ? "" : "1=1 OR ";
        var categoriesIDs = $"({string.Join(",", categories)})";
        var categoriesCount = searchCriteria.Categories?.Count() ?? 0;

        var pornstars = (searchCriteria.Pornstars?.Any() ?? false) ? searchCriteria.Pornstars.Select(p => p.ID) : [-1];
        var pornstarsFalse = (searchCriteria.Pornstars ?? []).Any() ? "" : "1=1 OR ";
        var pornstarsIDs = $"({string.Join(",", pornstars)})";
        var pornstarsCount = searchCriteria.Pornstars?.Count() ?? 0;

        var offset = searchCriteria.Paging.ResultsPerPage * (searchCriteria.Paging.PageIndex - 1);

        var finalQuery = string.Format(
            _query, 
            tagsFalse, 
            tagsIDs, 
            tagsCount, 
            pornstarsFalse, 
            pornstarsIDs, 
            pornstarsCount,
            categoriesFalse,
            categoriesIDs,
            categoriesCount,
            _defaultOrder,
            searchCriteria.Paging.ResultsPerPage,
            offset);

        var finalCountQuery = string.Format(
            _countQuery,
            tagsFalse,
            tagsIDs,
            tagsCount,
            pornstarsFalse,
            pornstarsIDs,
            pornstarsCount,
            categoriesFalse,
            categoriesIDs,
            categoriesCount);

        List<Videos.Models.Video> videos = [];
        int globalCount = -1;

        using (MySqlCommand command = new(finalQuery, dbConnection))
        {
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Videos.Models.Video video = new()
                {
                    VideoID = reader.GetInt32("VideoID"),
                    Title = reader.IsDBNull("Title") ? null : reader.GetString("Title"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    Duration = reader.IsDBNull("Duration") ? null : reader.GetTimeSpan("Duration"),
                    Year = reader.IsDBNull("Year") ? null : reader.GetInt32("Year"),
                    Tags = reader.IsDBNull("Tags") ? null : reader.GetString("Tags"),
                    Categories = reader.IsDBNull("Categories") ? null : reader.GetString("Categories"),
                    Pornstars = reader.IsDBNull("Pornstars") ? null : reader.GetString("Pornstars"),
                    Links = reader.IsDBNull("Links") ? null : reader.GetString("Links"),
                    Pictures = reader.IsDBNull("Pictures") ? null : reader.GetString("Pictures"),
                };

                videos.Add(video);
            }
        }

        using (MySqlCommand command = new(finalCountQuery, dbConnection))
        {
            using MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            globalCount = reader.GetInt32("COUNT");
        }

        var convertedVideos = videos.Select(video => {
            try
            {
                return video.Adapt<Core.Videos.Models.Video>();
            }
            catch(Exception ex)
            {
                Console.WriteLine("error");
                throw ex;
            }
        });

        return new SearchResult
        {
            GlobalCount = globalCount,
            Count = convertedVideos.Count(),
            Videos = convertedVideos.ToList()
        };
    }
}
