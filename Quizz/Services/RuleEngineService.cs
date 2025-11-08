using Quizz.Enums;
using Quizz.Models;

namespace Quizz.Services;

public class RuleEngineService
{
    public bool EvaluateConditions(Conditions conditions, Node node)
    {
        var evaluations = conditions.Items.Select(item => EvaluateItem(item, node));

        return conditions.Operator switch
        {
            Operator.And => evaluations.All(r => r),
            Operator.Or => evaluations.Any(r => r),
            _ => false
        };
    }

    private bool EvaluateItem(Item item, Node node)
    {
        var value = node.Get<string>(item.Field);
        if (value == null)
        {
            return false;
        }
        
        return item.Rule switch
        {
            RuleType.Equals => string.Equals(value, item.Value, StringComparison.OrdinalIgnoreCase),
            RuleType.StartsWith => value.StartsWith(item.Value, StringComparison.OrdinalIgnoreCase),
            RuleType.EndsWith => value.EndsWith(item.Value, StringComparison.OrdinalIgnoreCase),
            RuleType.Contains => value.Contains(item.Value, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}