using System.Linq.Expressions;

namespace NetQueryBuilder.Conditions;

public interface ICondition
{
    internal ICondition? Parent { get; set; }
    EventHandler ConditionChanged { get; set; }
    Expression Compile();
}

public class LogicalCondition : ICondition
{
    private ExpressionType _logicalType;
    public ExpressionType LogicalType
    {
        get => _logicalType;
        set
        {
            _logicalType = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public ICondition? Parent { get; set; }
    private Expression _left;
    public Expression Left
    {
        get => _left;
        set
        {
            _left = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    private Expression _right;
    public Expression Right
    {
        get => _right;
        set
        {
            _right = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    private ExpressionType _expressionType;
    public ExpressionType ExpressionType
    {
        get => _expressionType;
        set
        {
            _expressionType = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public EventHandler ConditionChanged { get; set; }

    public Expression Compile()
    {
        return Expression.MakeBinary(ExpressionType, _left, _right);
    }
}

public class BlockCondition : ICondition
{
    private ICondition? _parent;
    public ICondition? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public IReadOnlyCollection<ICondition> Conditions => _children.AsReadOnly();
    private List<ICondition> _children = new();
    public ExpressionType ExpressionType { get;  }

    public EventHandler ConditionChanged { get; set; }

    public BlockCondition(IEnumerable<ICondition> children, ExpressionType expressionType, ICondition? parent = null)
    {
        _children.AddRange(children);
        foreach (var condition in _children)
        {
            condition.Parent = this;
            condition.ConditionChanged += ChildConditionChanged;
        }
        ExpressionType = expressionType;
        Parent = parent;
    }

    public void Add(ICondition condition)
    {
        _children.Add(condition);
        condition.Parent = this;
        condition.ConditionChanged += ChildConditionChanged;
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void Remove(ICondition condition)
    {
        _children.Remove(condition);
        condition.Parent = null;
        condition.ConditionChanged -= ChildConditionChanged;
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public Expression Compile()
    {
        var expressions = Conditions.Select(c => c.Compile()).ToList();

        Expression result = expressions.First();
        foreach (var expression in expressions.Skip(1))
        {
            result = Expression.MakeBinary(ExpressionType, result, expression);
        }

        return result;
    }
    
    public void Group(IEnumerable<ICondition> childrenToGroup)
    {
        var children = Conditions.Where(c => childrenToGroup.Contains(c)).ToList();
        if (children.Count == 0)
        {
            return;
        }

        var block = new BlockCondition(children, ExpressionType, this);

        foreach (var child in children)
        {
            _children.Remove(child);
            child.ConditionChanged -= ChildConditionChanged;
            child.Parent = block;
        }

        _children.Add(block);
        block.ConditionChanged += ChildConditionChanged;
        
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ChildConditionChanged(object? sender, EventArgs args)
    {
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Ungroup(IEnumerable<ICondition> childrenToUngroup)
    {
        var blocks = Conditions.OfType<BlockCondition>().Where(b => childrenToUngroup.Contains(b)).ToList();
        if (blocks.Count == 0)
        {
            return;
        }

        foreach (var block in blocks)
        {
            _children.Remove(block);
            block.ConditionChanged -= ChildConditionChanged;
            foreach (var blockCondition in block.Conditions)
            {
                blockCondition.ConditionChanged -= block.ChildConditionChanged;
            }
            _children.AddRange(block.Conditions);
            foreach (var condition in block.Conditions)
            {
                condition.Parent = this;
                condition.ConditionChanged += ChildConditionChanged;
            }
        }
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CreateNew()
    {
        var lastLogicalCondition = FindLogicalCondition();
        var newLogicalCondition = new LogicalCondition
        {
            Parent = this,
            LogicalType = lastLogicalCondition.LogicalType,
            ExpressionType = lastLogicalCondition.ExpressionType,
            Left = lastLogicalCondition.Left,
            Right = lastLogicalCondition.Right
        };
        this.Add(newLogicalCondition);
        
    }
    private LogicalCondition FindLogicalCondition()
    {
        return Conditions.OfType<LogicalCondition>().FirstOrDefault() ?? Conditions.OfType<BlockCondition>().Select(c => c.FindLogicalCondition()).First();
    }
}