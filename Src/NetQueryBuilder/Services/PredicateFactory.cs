using System.Linq.Expressions;

namespace NetQueryBuilder.Services;

public class PredicateFactory
{
    public Expression<Func<T, bool>> CreateRelationalPredicate<T>(
        string propertyName,
        ParameterExpression parameter,
        object comparisonValue,
        ExpressionType expressionType)
    {
        var property = typeof(T).GetProperty(propertyName);
        var memberAccess = Expression.MakeMemberAccess(parameter, property);

        var right = Expression.Constant(comparisonValue);

        var binary = Expression.MakeBinary(expressionType, memberAccess, right);

        Expression<Func<T, bool>> expression = Expression.Lambda(binary, parameter) as Expression<Func<T, bool>>;

        return expression;
    }
}