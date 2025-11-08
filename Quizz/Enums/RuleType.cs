using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Quizz.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<RuleType>))]
public enum RuleType
{
    [Description("Exact match between field and value")]
    Equals,
    [Description("Field value must start with the specified value")]
    StartsWith,
    [Description("Field value must end with the specified value")]
    EndsWith,
    [Description("Field value must contain with the specified value")]
    Contains
}