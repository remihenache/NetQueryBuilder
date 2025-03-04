using NetQueryBuilder.Util;

namespace NetQueryBuilder.Operators;

public class InListOperator<T> : MethodCallOperator
{
    public override string DisplayText => IsNegated ? "Not in list" : "In list";
    public InListOperator(bool isNegated = false)
        : base(EnumerableMethodInfo.Contains<T>(), isNegated)
    {
    }
}