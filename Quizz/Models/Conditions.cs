using System.ComponentModel;
using System.Text.Json.Serialization;
using Quizz.Enums;

namespace Quizz.Models;

public sealed record Conditions(
    [property: JsonPropertyName("conditions"), Description("Determines how multiple items are combined")]
    Operator Operator,
    [property: JsonPropertyName("items"), Description("A list of individual matching rules")]
    List<Item> Items
);