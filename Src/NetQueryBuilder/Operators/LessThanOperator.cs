using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public class LessThanOperator : BinaryOperator
{
    public override ExpressionType ExpressionType => ExpressionType.LessThan;
    public override string DisplayText => "Less than";
}