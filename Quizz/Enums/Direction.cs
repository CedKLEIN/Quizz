using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Quizz.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<Direction>))]
[Description("Defines how the question and answer are presented to the user")]
public enum Direction
{
    [Description("Question is shown first, user must provide the answer")]
    Forward,

    [Description("Answer is shown first, user must provide the question")]
    Reverse,

    [Description("Randomly switches between forward and reverse modes")]
    Random
}