using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Services;

namespace NetQueryBuilder.EntityFramework;

public static class QueryBuilderServicesExtensions
{
    public static IServiceCollection AddQueryBuilderServices<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        services.AddTransient(typeof(QueryBuilderService<>));
        services.AddSingleton<PredicateFactory>();

        services.AddTransient<DefaultDynamicLinqCustomTypeProvider>(_ => new CustomEfTypeProvider(new ParsingConfig
        {
            RenameParameterExpression = true
        }, true));

        services.AddTransient<IQueryServiceFactory, QueryServiceFactory<TDbContext>>();

        var dbContextType = typeof(TDbContext);
        var dbSetProperties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var dbSetProperty in dbSetProperties)
        {
            var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
            var queryServiceType = typeof(EFQueryService<,>).MakeGenericType(entityType, dbContextType);
            services.AddTransient(queryServiceType);
        }

        return services;
    }
}