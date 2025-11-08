using System.ComponentModel;
using System.Text.Json.Serialization;
using Quizz.Enums;

namespace Quizz.Models;

public sealed record Item(
    [property:JsonPropertyName("field"),Description("The property or key being matched")]
    string Field,
    [property:JsonPropertyName("value"), Description("The property or key being matched")]
    string Value,
    [property:JsonPropertyName("rule"), Description("how to compare the actual and expected values")]
    RuleType Rule
    );