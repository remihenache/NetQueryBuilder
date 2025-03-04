using System.Collections;
using System.Linq.Expressions;

namespace NetQueryBuilder.Services;

public interface IQueryService<T> where T: class
{
    Task<IEnumerable> QueryData(string predicateExpression, IEnumerable<string> selectedProperties);
    Task<IEnumerable> QueryData(Expression<Func<T, bool>> predicate, IEnumerable<string> selectedProperties);
}