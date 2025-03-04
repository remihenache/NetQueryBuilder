using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public class GreaterThanOrEqualOperator : BinaryOperator
{
    public override ExpressionType ExpressionType => ExpressionType.GreaterThanOrEqual;
    public override string DisplayText => "Greater than or equal";
}