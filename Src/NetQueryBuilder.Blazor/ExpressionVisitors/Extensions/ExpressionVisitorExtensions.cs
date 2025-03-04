using System.Linq.Expressions;

namespace NetQueryBuilder.Blazor.ExpressionVisitors.Extensions;

public static class ExpressionVisitorExtensions
{
    public static Expression Copy(this Expression expression)
    {
        return new CopyExpression(expression).Execute();
    }
}