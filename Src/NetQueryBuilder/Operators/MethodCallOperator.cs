using System.Linq.Expressions;
using System.Reflection;

namespace NetQueryBuilder.Operators;

public class MethodCallOperator : ExpressionOperator
{
    public override ExpressionType ExpressionType => ExpressionType.Call;
    public MethodInfo MethodInfo { get; set; }
    public bool IsNegated { get; set; }

    public MethodCallOperator(MethodInfo methodInfo, bool isNegated = false)
    {
        MethodInfo = methodInfo;
        IsNegated = isNegated;
    }

    public override bool Equals(object obj)
    {
        return obj is MethodCallOperator op
               && base.Equals(obj)
               && EqualityComparer<MethodInfo>.Default.Equals(MethodInfo, op.MethodInfo)
               && IsNegated == op.IsNegated;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), MethodInfo, IsNegated);
    }

    public static bool operator ==(MethodCallOperator left, MethodCallOperator right)
    {
        return EqualityComparer<MethodCallOperator>.Default.Equals(left, right);
    }

    public static bool operator !=(MethodCallOperator left, MethodCallOperator right)
    {
        return !(left == right);
    }
}