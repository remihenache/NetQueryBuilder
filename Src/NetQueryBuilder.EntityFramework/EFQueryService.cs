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
        try
        {
            predicateExpression = predicateExpression
                .Replace("AndAlso", "&&")
                .Replace("OrElse", "||");
            
            var types = _dbContext
                .Model
                .GetEntityTypes()
                .Select(t => t.ClrType.Namespace)
                .Concat(new []{
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
        catch (Exception ex)
        {

            throw;
        }
    }
    public async Task<IEnumerable> QueryData(
        Expression<Func<T, bool>> predicate,
        IEnumerable<string> selectedProperties)
    {
        // 1. Construire la requête de base
        IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

        // 2. Déterminer les chemins de navigation
        var navigationPaths = ExtractNavigationPaths(selectedProperties);
        foreach (var path in navigationPaths)
        {
            query = query.Include(path);
        }

        // 3. Appliquer le filtre
        query = query.Where(predicate);
        var select = new SelectBuilderService<T>().BuildSelect(selectedProperties);
        query = query.Select(select);

        return await query.ToListAsync();
    }

    // Ici, on isole la partie qui extrait les chemins "Address" ou "Address.City", etc.
    // On peut même faire un peu de filtrage pour ne pas inclure deux fois le même chemin.
    private IEnumerable<string> ExtractNavigationPaths(IEnumerable<string> selectedProperties)
    {
        // Récupérer la partie qui correspond à la navigation (tout sauf la dernière)
        // ex: "Address.City" => "Address"
        // ex: "Address.Country.Name" => "Address.Country"
        // Là, on peut également renvoyer les chemins complets (EF gère la notation pointée).
        // Selon le besoin, vous pouvez affiner la logique.

        var paths = new HashSet<string>();

        foreach(var propPath in selectedProperties)
        {
            // "Address.City" peut être incluse directement => "Address.City"
            // ou on pourrait vouloir juste "Address"
            // EF supporte la notation "Address.City", donc on peut l’ajouter tel quel.
            // Mais si vous voulez gérer ThenInclude, il faudra une logique plus complexe.
            
            if (propPath.Contains("."))
            {
                // On récupère avant-dernier segment pour un ThenInclude
                // Pour simplifier l’exemple, on inclut le chemin entier :
                paths.Add(propPath.Substring(0, propPath.LastIndexOf('.')));
            }
        }

        return paths;
    }
}