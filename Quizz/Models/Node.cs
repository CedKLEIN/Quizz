using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Quizz.Enums;

namespace Quizz.Models;

public sealed record Node(
    [property: Description("name"), JsonPropertyName("name")] string Name,
    [property: Description("label"), JsonPropertyName("label")] string Label,
    [property: Description("type"), JsonPropertyName("type")] PageType Type
)
{
    [JsonExtensionData]
    public IDictionary<string, JsonElement> Properties { get; init; } = new Dictionary<string, JsonElement>();

    public string? Question => Get<string>("question");
    public string? Answer => Get<string>("answer");
    
    public Conditions? Conditions => Get<Conditions>("conditions");
    
    public T? Get<T>(string propertyName)
    {
        if (!Properties.TryGetValue(propertyName, out var element)) return default;
        try
        {
            return element.Deserialize<T>();
        }
        catch
        {
            return default;
        }
    }
    
    [JsonIgnore]
    public List<Node> Children = [];
}
