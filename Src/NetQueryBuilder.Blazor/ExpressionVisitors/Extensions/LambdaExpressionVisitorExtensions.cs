using System.Linq.Expressions;

namespace NetQueryBuilder.Blazor.ExpressionVisitors.Extensions;

public static class LambdaExpressionVisitorExtensions
{
    public static LambdaExpression ReplaceBody(this LambdaExpression expression, Expression newBody)
    {
        return new ReplaceLambdaBody(expression, newBody).Execute();
    }
}