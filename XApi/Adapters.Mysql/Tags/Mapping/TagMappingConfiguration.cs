using Mapster;

namespace XApi.Adapters.Mysql.Tags.Mapping;

public class TagMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Tag, Core.Tags.Models.Tag>()
            .Map(dest => dest.ID, src => src.ID)
            .Map(dest => dest.Value, src => src.Value!.ToLower())
            .Map(dest => dest.Count, src => src.Count)
            .Map(dest => dest.RecentCount, src => src.RecentCount);
    }
}
