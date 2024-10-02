using Dapper;
using Mapster;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using XApi.Adapters.Mysql.Videos.Models;
using XApi.Core.Links.Enums;
using XApi.Core.Pictures.Models;
using XApi.Core.Search.Models;
using XApi.Core.Search.Ports.Interfaces;
using XApi.Core.Videos.Models;

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
AND v.ID = 69
        GROUP BY 
            v.ID, v.Title, v.Description, v.Duration, v.Year, v.CreatedDate, v.ModifiedDate
        ORDER BY
	        v.ModifiedDate {6}
        LIMIT {7} OFFSET {8}
    ";

    private readonly int _defaultLimit = 10;
    private readonly int _defaultOffset = 0;
    private readonly string _defaultOrder = "DESC";
            

    private MySqlConnection Connection => new MySqlConnection(_connectionString);

    public SearchProvider()
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
    }

    public async Task<SearchResult> SearchVideosByCriteria(SearchCriteria searchCriteria)
    {
        using MySqlConnection dbConnection = Connection;
        dbConnection.Open();

        var tags = (searchCriteria?.TagsIDS?.Any() ?? false) ? searchCriteria.TagsIDS : [-1];
        var tagsFalse = (searchCriteria?.TagsIDS ?? []).Any() ? "" : "1=1 OR ";
        var tagsIDs = $"({string.Join(",", tags)})";
        var tagsCount = searchCriteria?.TagsIDS?.Count() ?? 0;

        var pornstars = (searchCriteria?.PornstarsIDS?.Any() ?? false) ? searchCriteria.PornstarsIDS : [-1];
        var pornstarsFalse = (searchCriteria?.PornstarsIDS ?? []).Any() ? "" : "1=1 OR ";
        var pornstarsIDs = $"({string.Join(",", pornstars)})";
        var pornstarsCount = searchCriteria?.PornstarsIDS?.Count() ?? 0;

        var finalQuery = string.Format(
            _query, 
            tagsFalse, 
            tagsIDs, 
            tagsCount, 
            pornstarsFalse, 
            pornstarsIDs, 
            pornstarsCount,
            _defaultOrder,
            _defaultLimit,
            _defaultOffset);

        List<Videos.Models.Video> videos = [];

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
                    Pornstars = reader.IsDBNull("Pornstars") ? null : reader.GetString("Pornstars"),
                    Links = reader.IsDBNull("Links") ? null : reader.GetString("Links"),
                    Pictures = reader.IsDBNull("Pictures") ? null : reader.GetString("Pictures"),
                };

                videos.Add(video);
            }
        }

        var convertedVideos = videos.Select(video => {
            try
            {
                return new Core.Videos.Models.Video
                {
                    ID = video.VideoID,
                    Title = video.Title,
                    Description = video.Description,
                    Duration = video.Duration,
                    Year = video.Year,
                    Tags = video.Tags?
                    .Split('|')
                    .Select(tag =>
                    {
                        var splittedTag = tag.Split('µ');
                        return new Core.Tags.Models.Tag
                        {
                            ID = GetIntValue(splittedTag[0])!.Value,
                            Value = GetStrValue(splittedTag[1])
                        };
                    })
                    .ToList() ?? [],
                    Pornstars = video.Pornstars?
                    .Split('|')
                    .Select(pornstar =>
                    {
                        var splittedPornstar = pornstar.Split('µ');
                        return new Core.Pornstars.Models.Pornstar
                        {
                            ID = GetIntValue(splittedPornstar[0])!.Value,
                            Value = GetStrValue(splittedPornstar[1])
                        };
                    })
                    .ToList() ?? [],
                    Links = video.Links?
                    .Split('|')
                    .Select(link =>
                    {
                        var splittedLink = link.Split('µ');
                        return new Core.Links.Models.Link
                        {
                            Url = GetStrValue(splittedLink[0]),
                            Size = GetDoubleValue(splittedLink[1]),
                            Host = GetValue<Host>(splittedLink[2]),
                            Resolution = GetValue<Resolution>(splittedLink[3]),
                            Format = GetValue<Format>(splittedLink[4]),
                            Part = GetIntValue(splittedLink[5])
                        };
                    })
                    .ToList() ?? [],
                    Pictures = video.Pictures?
                    .Split('|')
                    .Select(picture =>
                    {
                        var splittedPicture = picture.Split('µ');
                        return new Picture
                        {
                            DirectUrl = GetStrValue(splittedPicture[0]),
                            HostUrl = GetStrValue(splittedPicture[1])
                        };
                    })
                    .ToList() ?? [],
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine("error");
                throw ex;
            }
        });


        return new SearchResult
        {
            Videos = convertedVideos.ToList()
        };
    }

    private string GetStrValue(string value)
    {
        if (value == "NULL") return null;
        return value;
    }

    private int? GetIntValue(string value)
    {
        if (value == "NULL") return null;
        return int.TryParse(value, out int part) ? part : null;
    }

    private double? GetDoubleValue(string value)
    {
        if (value == "NULL") return null;
        return double.TryParse(value, out double part) ? part : null;
    }

    private T? GetValue<T>(string enumValue) where T : Enum
    {
        if (enumValue == "NULL") return default;
        var parsed = int.TryParse(enumValue, out var value);
        if (!parsed || !Enum.IsDefined(typeof(T), value)) return default;

        return (T)(object)value;
    }
}
