using XApi.Core.Search.Models;

namespace XApi.Core.Seo.Builders.Interfaces;

public interface IDescriptionBuilder
{
    public string BuildFrom(SearchCriteria criteria);
}
