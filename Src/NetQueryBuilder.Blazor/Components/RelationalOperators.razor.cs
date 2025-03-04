using NetQueryBuilder.EntityFramework.Operators;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Blazor.Components;

public partial class RelationalOperators
{
    public static List<ExpressionOperator> GetOperators(Type operandType)
    {
        return operandType switch
        {
            Type type when type == typeof(int) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                new LessThanOperator(),
                new LessThanOrEqualOperator(),
                new GreaterThanOperator(),
                new GreaterThanOrEqualOperator(),
                new InListOperator<int>(),
                new InListOperator<int>(true)
            },
            Type type when type == typeof(string) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                new EfLikeOperator(),
                new EfLikeOperator(true),
                new InListOperator<string>(),
                new InListOperator<string>(true)
            },
            Type type when type == typeof(bool) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator()
            },
            Type type when type == typeof(DateTime) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                new LessThanOperator(),
                new LessThanOrEqualOperator(),
                new GreaterThanOperator(),
                new GreaterThanOrEqualOperator()
            },
            _ => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator()
            }
        };
    }
}