using System.Linq.Expressions;

namespace NetQueryBuilder.Blazor.ExpressionVisitors;

public class ChangePropertyAccess : ExpressionVisitor, IExpressionVisitor<MemberExpression>
{
    private readonly Expression _expression;
    private readonly string _propertyName;
    private readonly Type _propertyType;

    internal ChangePropertyAccess(Expression expression, Type propertyType, string propertyName)
    {
        _expression = expression;
        _propertyName = propertyName;
        _propertyType = propertyType;
    }

    public MemberExpression Execute()
    {
        return (MemberExpression)Visit(_expression);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node == _expression)
        {
            var expression = Visit(node.Expression);
            var prop = _propertyType.GetProperty(_propertyName);
            return Expression.MakeMemberAccess(expression, prop);
        }

        return base.VisitMember(node);
    }
}