using Mapster;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using System.Data;
using XApi.Adapters.Mysql.Videos.Models;
using XApi.Core.Videos.Ports.Interfaces;

namespace XApi.Adapters.Mysql.Search.Adapters;

public class VideoProvider : IVideoProvider
{
    private readonly string _connectionString;
    private readonly IMemoryCache _cache;
    private readonly string _cacheKey = "VideoId{0}";
    private readonly TimeSpan _cacheDuration = TimeSpan.FromDays(1);
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
        WHERE v.ID = {0}
    ";

    private MySqlConnection Connection => new MySqlConnection(_connectionString);

    public VideoProvider(IMemoryCache cache)
    {
        _connectionString = Environment.GetEnvironmentVariable("BDD_CONNECTION_STRING")
            ?? throw new InvalidOperationException("La variable d'environnement 'BDD_CONNECTION_STRING' est manquante.");
        _cache = cache;
    }

    public async Task<Core.Videos.Models.Video?> ProvideVideoForId(int id)
    {
        if (id == null) throw new Exception("The id should not be null.");

        if (!_cache.TryGetValue(string.Format(_cacheKey, id), out Core.Videos.Models.Video? video))
        {
            using var dbConnection = Connection;
            dbConnection.Open();

            var finalQuery = string.Format(_query, id);

            List<Video> videos = [];

            using (MySqlCommand command = new(finalQuery, dbConnection))
            {
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Video createdVideo = new()
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

                    videos.Add(createdVideo);
                }
            }

            video = videos.Select(video => {
                try
                {
                    return video.Adapt<Core.Videos.Models.Video>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error");
                    throw ex;
                }
            }).FirstOrDefault();

            if (video != null)
                _cache.Set(string.Format(_cacheKey, id), video, _cacheDuration);
        }
        
        return video;
    }
}
