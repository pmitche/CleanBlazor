using System.Text.Json;
using BlazorHero.CleanArchitecture.Application.Abstractions.Serialization;
using BlazorHero.CleanArchitecture.Application.Serialization.JsonConverters;

namespace BlazorHero.CleanArchitecture.Application.Serialization;

public class SystemTextJsonSerializer : IJsonSerializer
{
    private static readonly JsonSerializerOptions s_defaultOptions = new()
    {
        Converters = { new TimespanJsonConverter() }
    };

    public T Deserialize<T>(string text)
        => JsonSerializer.Deserialize<T>(text, s_defaultOptions);

    public string Serialize<T>(T obj)
        => JsonSerializer.Serialize(obj, s_defaultOptions);
}
