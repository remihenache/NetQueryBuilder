using System.Linq.Expressions;

namespace NetQueryBuilder.Blazor.ExpressionVisitors;

public interface IExpressionVisitor<T> where T : Expression
{
    T Execute();
}