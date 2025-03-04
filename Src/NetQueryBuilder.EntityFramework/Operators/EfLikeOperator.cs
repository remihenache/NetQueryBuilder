using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.EntityFramework.Operators;

public class EfLikeOperator : MethodCallOperator
{
    public EfLikeOperator(bool isNegated = false)
        : base(typeof(DbFunctionsExtensions).GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) }), isNegated)
    {
    }

    public override string DisplayText => IsNegated ? "Not like" : "Like";
}