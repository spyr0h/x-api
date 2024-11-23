using Mapster;

namespace XApi.Adapters.Mysql.Pornstars.Mapping;

public class PornstarMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Pornstar, Core.Pornstars.Models.Pornstar>()
            .Map(dest => dest.ID, src => src.ID)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.Count, src => src.Count)
            .Map(dest => dest.RecentCount, src => src.RecentCount);
    }
}
