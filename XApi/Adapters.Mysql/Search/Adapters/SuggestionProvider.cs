using Mapster;
using MySql.Data.MySqlClient;
using System.Data;
using XApi.Core.Suggestion.Ports.Interfaces;
using XApi.Core.Videos.Models;

namespace XApi.Adapters.Mysql.Search.Adapters;

public class SuggestionProvider : ISuggestionProvider
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
                    FROM PornstarVideo 
                    WHERE PornstarsID IN {1}
                    GROUP BY VideosID 
                )
            )
        OR 
            (
                {2}
                v.ID IN (
                    SELECT VideosID 
                    FROM TagVideo 
                    WHERE TagsID IN {3}
                    GROUP BY VideosID 
                )
            )
        OR 
            (
                {4}
                v.ID IN (
                    SELECT VideosID 
                    FROM CategoryVideo 
                    WHERE CategoriesID IN {5}
                    GROUP BY VideosID 
                )
            )
        GROUP BY 
            v.ID, v.Title, v.Description, v.Duration, v.Year, v.CreatedDate, v.ModifiedDate
        ORDER BY
	        v.ModifiedDate {6}
        LIMIT 100
    ";

    private readonly string _defaultOrder = "DESC";

    private MySqlConnection Connection => new MySqlConnection(_connectionString);

    public SuggestionProvider()
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
    }

    public async Task<Video[]> ProvideSuggestedVideos(Video video)
    {
        if (video == null) throw new Exception("The video should not be null.");

        using MySqlConnection dbConnection = Connection;
        dbConnection.Open();

        var tags = (video.Tags?.Any() ?? false) ? video.Tags.Select(t => t.ID) : [-1];
        var tagsFalse = (video.Tags ?? []).Any() ? "" : "1!=1 AND ";
        var tagsIDs = $"({string.Join(",", tags)})";

        var categories = (video.Categories?.Any() ?? false) ? video.Categories.Select(t => t.ID) : [-1];
        var categoriesFalse = (video.Categories ?? []).Any() ? "" : "1!=1 AND ";
        var categoriesIDs = $"({string.Join(",", categories)})";

        var pornstars = (video.Pornstars?.Any() ?? false) ? video.Pornstars.Select(p => p.ID) : [-1];
        var pornstarsFalse = (video.Pornstars ?? []).Any() ? "" : "1!=1 AND ";
        var pornstarsIDs = $"({string.Join(",", pornstars)})";

        var finalQuery = string.Format(
            _query,
            tagsFalse,
            tagsIDs,
            pornstarsFalse,
            pornstarsIDs,
            categoriesFalse,
            categoriesIDs,
            _defaultOrder);

        List<Videos.Models.Video> videos = [];
        int globalCount = -1;

        using (MySqlCommand command = new(finalQuery, dbConnection))
        {
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Videos.Models.Video foundVideo = new()
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

                videos.Add(foundVideo);
            }
        }

        var convertedVideos = videos.Select(video => {
            try
            {
                return video.Adapt<Video>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
                throw ex;
            }
        });

        return convertedVideos.ToArray();
    }
}
