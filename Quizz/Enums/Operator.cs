using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Quizz.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<Operator>))]
public enum Operator
{
   [Description("Logical OR operator: at least one condition must be met")]
   Or,
   [Description("Logical AND operator: all conditions must be met")]
   And
}