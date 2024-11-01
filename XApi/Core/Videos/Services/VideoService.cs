using XApi.Core.Videos.Models;
using XApi.Core.Videos.Ports.Interfaces;

namespace XApi.Core.Videos.Services;

public class VideoService(IVideoProvider videoProvider) : IVideoService
{
    public Task<Video?> ProvideVideoForId(int id)
        => videoProvider.ProvideVideoForId(id);
}
