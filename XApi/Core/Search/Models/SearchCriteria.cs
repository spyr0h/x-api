﻿namespace XApi.Core.Search.Models;

public record SearchCriteria
{
    public List<int> TagsIDS { get; set; } = [];
    public List<int> PornstarsIDS { get; set; } = [];
    public SearchPaging Paging { get; set; } = new() { PageIndex = 1 };
}
