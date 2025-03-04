namespace NetQueryBuilder.Services;

public interface IQueryServiceFactory
{
    Task<IEnumerable<Type>> GetEntities();
    Task<IEnumerable<string>> GetProperties(Type? entityType = null);
    IQueryService<T> Create<T>() where T : class;
}