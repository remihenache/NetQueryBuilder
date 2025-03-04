using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public class GreaterThanOperator : BinaryOperator
{
    public override ExpressionType ExpressionType => ExpressionType.GreaterThan;
    public override string DisplayText => "Greater than";
}