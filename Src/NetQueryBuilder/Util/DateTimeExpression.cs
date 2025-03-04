using System.Linq.Expressions;

namespace NetQueryBuilder.Util;

public static class DateTimeExpression
{
    public static NewExpression New(DateTime dateTime)
    {
        var ticksExpression = Expression.Constant(dateTime.Ticks, typeof(long));
        return Expression.New(typeof(DateTime).GetConstructor([typeof(long)])!, ticksExpression);
    }
}