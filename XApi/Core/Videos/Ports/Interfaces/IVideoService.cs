using XApi.Core.Videos.Models;

namespace XApi.Core.Videos.Ports.Interfaces;

public interface IVideoService
{
    Task<Video?> ProvideVideoForId(int id);
}