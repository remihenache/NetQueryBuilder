using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public class EqualsOperator : BinaryOperator
{
    public override ExpressionType ExpressionType => ExpressionType.Equal;
    public override string DisplayText => "Equals";
}