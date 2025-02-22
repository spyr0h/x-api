﻿namespace XApi.Core.Paging.Models;

public record SearchPaging
{
    public List<SearchPage> Pages { get; set; } = [];
    public SearchPage? PreviousPage { get; set; }
    public SearchPage? NextPage { get; set; }
}
