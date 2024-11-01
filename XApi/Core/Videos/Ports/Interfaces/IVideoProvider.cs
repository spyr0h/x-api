using XApi.Core.Videos.Models;

namespace XApi.Core.Videos.Ports.Interfaces;

public interface IVideoProvider
{
    Task<Video?> ProvideVideoForId(int id);
}