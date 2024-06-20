﻿using System;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace ValorantPorting.ViewModels.Endpoints.Models;

public class MappingsResponse
{
    [J] public string URL;
    [J] public string Filename;
    [J] public long Length;
    [J] public DateTime Uploaded;
    [J] public MappingsMeta Meta;
}

public class MappingsMeta
{
    [J] public string Version;
    [J] public string CompressionMethod;
}

public class ValorantPortingMappingsResponse
{
    [J] public bool Active;
    [J] public MappingsResponse[] Mappings;
}