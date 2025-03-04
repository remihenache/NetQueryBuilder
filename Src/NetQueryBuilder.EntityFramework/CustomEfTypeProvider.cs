using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Services;

namespace NetQueryBuilder.EntityFramework;

public class CustomEfTypeProvider : DefaultDynamicLinqCustomTypeProvider
{
    public CustomEfTypeProvider(ParsingConfig config, bool cache) : base(config, cache)
    {
    }

    public override HashSet<Type> GetCustomTypes()
    {
        var customTypes = base.GetCustomTypes();

        customTypes.Add(typeof(EF));
        customTypes.Add(typeof(DbFunctions));
        customTypes.Add(typeof(DbFunctionsExtensions));
            
        return customTypes;
    }
}


public class QueryServiceFactory<TDbContext> : IQueryServiceFactory
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public QueryServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<IEnumerable<Type>> GetEntities()
    {
        return Task.FromResult<IEnumerable<Type>>(_serviceProvider.GetRequiredService<TDbContext>()
            .Model
            .GetEntityTypes()
            .Select(t => t.ClrType)
            .ToList());
    }
    public Task<IEnumerable<string>> GetProperties(Type? entityType = null)
    {
        return Task.FromResult<IEnumerable<string>>(entityType.GetProperties().Select(p => p.Name).ToList());
    }

    public IQueryService<T> Create<T>() where T : class
    {
        var service = _serviceProvider.GetRequiredService(typeof(EFQueryService<,>).MakeGenericType(typeof(T), typeof(TDbContext)));

        return service as IQueryService<T>;
    }
}