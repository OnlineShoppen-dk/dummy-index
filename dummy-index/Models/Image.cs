﻿namespace dummy_index.Models;

public class Image
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string Alt { get; set; } = null!;
}

public class ElasticSearchImage
{
    public int id { get; set; }
    public string name { get; set; } = null!;
    public string fileName { get; set; } = null!;
    public string alt { get; set; } = null!;
}