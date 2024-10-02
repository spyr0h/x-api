using Mapster;
using XApi.API.Pictures.DTO;
using XApi.Core.Pictures.Models;

namespace XApi.API.Pictures.Mapping;

public class PictureMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Picture, PictureDTO>()
            .Map(dest => dest.DirectUrl, src => src.DirectUrl)
            .Map(dest => dest.HostUrl, src => src.HostUrl);
    }
}
