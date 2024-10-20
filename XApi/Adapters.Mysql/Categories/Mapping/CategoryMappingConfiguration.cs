using Mapster;

namespace XApi.Adapters.Mysql.Categories.Mapping;

public class CategoryMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Category, Core.Categories.Models.Category>()
            .Map(dest => dest.ID, src => src.ID)
            .Map(dest => dest.Value, src => src.Value);
    }
}
