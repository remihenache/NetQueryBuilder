using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public class LessThanOrEqualOperator : BinaryOperator
{
    public override ExpressionType ExpressionType => ExpressionType.LessThanOrEqual;
    public override string DisplayText => "Less than or equal";
}