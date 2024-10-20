using Mapster;
using XApi.API.Categories.DTO;
using XApi.Core.Categories.Models;

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
