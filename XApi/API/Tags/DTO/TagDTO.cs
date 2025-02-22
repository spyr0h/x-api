﻿namespace XApi.API.Tags.DTO;

public record TagDTO
{
    public int ID { get; set; }
    public string? Value { get; set; }
    public int Count { get; set; }
    public int RecentCount { get; set; }
}
