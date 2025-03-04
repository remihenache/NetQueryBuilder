using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using NetQueryBuilder.Extensions;

namespace NetQueryBuilder.Services;

public class QueryBuilderService<TEntity>
{
    private readonly DefaultDynamicLinqCustomTypeProvider _customTypeProvider;

    public QueryBuilderService(DefaultDynamicLinqCustomTypeProvider customTypeProvider)
    {
        _customTypeProvider = customTypeProvider;
    }

    public LambdaExpression? Lambda { get; set; }
    public ParameterExpression? Parameter { get; set; }

    public void LoadEntity()
    {
        Parameter = Expression.Parameter(
            typeof(TEntity),
            typeof(TEntity).Name.ToLower());

        Lambda = new PredicateFactory().CreateRelationalPredicate<TEntity>(
            typeof(TEntity).GetProperties().First().Name,
            Parameter,
            typeof(TEntity).GetProperties().First().PropertyType.GetDefaultValue(),
            ExpressionType.Equal);
    }

    public void LoadQuery(string expression)
    {
        // var config = new ParsingConfig { RenameParameterExpression = true };
        // config.CustomTypeProvider = new CustomEFTypeProvider(config, true);
        var config = new ParsingConfig { RenameParameterExpression = true };
        config.CustomTypeProvider = _customTypeProvider;

        Lambda = DynamicExpressionParser.ParseLambda(
            config,
            typeof(TEntity),
            typeof(bool),
            expression);

        Parameter = Lambda.Parameters[0];
    }
}