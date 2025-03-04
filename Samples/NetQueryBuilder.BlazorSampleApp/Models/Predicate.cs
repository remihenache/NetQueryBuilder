using System.Linq.Expressions;

namespace NetQueryBuilder.BlazorSampleApp.Models;

public class Predicate
{
    public string LambdaExpression { get; set; }
    public List<string> SelectedProperties { get; set; }
    public string EntityName { get; set; }
    public string EntityType { get; set; }
    public Expression Expression { get; set; }
}