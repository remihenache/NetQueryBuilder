using System.Collections;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Services;

namespace NetQueryBuilder.EntityFramework;

public class EFQueryService<T, TDbContext> : IQueryService<T> where T : class where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public EFQueryService(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable> QueryData(string predicateExpression, IEnumerable<string> selectedProperties)
    {
        predicateExpression = predicateExpression
            .Replace("AndAlso", "&&")
            .Replace("OrElse", "||");

        var types = _dbContext
            .Model
            .GetEntityTypes()
            .Select(t => t.ClrType.Namespace)
            .Concat(new[]
            {
                _dbContext.GetType().Namespace,
                "System"
            })
            .Distinct()
            .ToList();

        var options = ScriptOptions
            .Default
            .AddReferences(typeof(T).Assembly)
            .AddImports(types);

        Expression<Func<T, bool>> predicate = await CSharpScript.EvaluateAsync<Expression<Func<T, bool>>>(predicateExpression, options);

        return await QueryData(predicate, selectedProperties);
    }

    public async Task<IEnumerable> QueryData(
        Expression<Func<T, bool>> predicate,
        IEnumerable<string> selectedProperties)
    {
        IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
        var navigationPaths = ExtractNavigationPaths(selectedProperties);
        foreach (var path in navigationPaths) query = query.Include(path);

        query = query.Where(predicate);
        var select = new SelectBuilderService<T>().BuildSelect(selectedProperties);
        query = query.Select(select);

        return await query.ToListAsync();
    }

     private IEnumerable<string> ExtractNavigationPaths(IEnumerable<string> selectedProperties)
    {
        var paths = new HashSet<string>();

        foreach (var propPath in selectedProperties)
            // "Address.City" peut être incluse directement => "Address.City"
            // ou on pourrait vouloir juste "Address"
            // EF supporte la notation "Address.City", donc on peut l’ajouter tel quel.
            // Mais si vous voulez gérer ThenInclude, il faudra une logique plus complexe.
            if (propPath.Contains("."))
                // On récupère avant-dernier segment pour un ThenInclude
                // Pour simplifier l’exemple, on inclut le chemin entier :
                paths.Add(propPath.Substring(0, propPath.LastIndexOf('.')));

        return paths;
    }
}