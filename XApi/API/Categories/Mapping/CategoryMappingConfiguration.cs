using Mapster;
using XApi.Adapters.Mysql.Categories.Models;
using XApi.API.Categories.DTO;

namespace XApi.API.Categories.Mapping;

public class CategoryMappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDTO>()
             .Map(dest => dest.ID, src => src.ID)
             .Map(dest => dest.Value, src => src.Value);
    }
}
