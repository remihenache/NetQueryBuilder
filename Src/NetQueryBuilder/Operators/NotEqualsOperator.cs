using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public class NotEqualsOperator : BinaryOperator
{
    public override ExpressionType ExpressionType => ExpressionType.NotEqual;
    public override string DisplayText => "Does not equal";
}