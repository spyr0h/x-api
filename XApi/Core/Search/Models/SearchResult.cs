﻿using XApi.Core.Videos.Models;

namespace XApi.Core.Search.Models;

public record SearchResult
{
    public int Count { get; set; }
    public List<Video> Videos { get; set; } = []; 
}
