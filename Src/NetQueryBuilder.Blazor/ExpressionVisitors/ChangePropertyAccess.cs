﻿using System.Linq.Expressions;
using System.Reflection;

namespace NetQueryBuilder.Blazor.ExpressionVisitors;

public class ChangePropertyAccess : ExpressionVisitor, IExpressionVisitor<MemberExpression>
{
    private readonly Expression _expression;
    private readonly Type _propertyType;
    private readonly string _propertyName;

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
            Expression expression = Visit(node.Expression);
            PropertyInfo prop = _propertyType.GetProperty(_propertyName);
            return Expression.MakeMemberAccess(expression, prop);
        }
        return base.VisitMember(node);
    }
}