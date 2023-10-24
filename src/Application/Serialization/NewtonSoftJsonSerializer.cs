using BlazorHero.CleanArchitecture.Application.Abstractions.Serialization;
using Newtonsoft.Json;

namespace BlazorHero.CleanArchitecture.Application.Serialization;

public class NewtonSoftJsonSerializer : IJsonSerializer
{
    private static readonly JsonSerializerSettings s_defaultSettings = new();

    public T Deserialize<T>(string text)
        => JsonConvert.DeserializeObject<T>(text, s_defaultSettings);

    public string Serialize<T>(T obj)
        => JsonConvert.SerializeObject(obj, s_defaultSettings);
}
