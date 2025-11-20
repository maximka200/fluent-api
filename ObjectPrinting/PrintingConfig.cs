using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting;

public class PrintingConfig<TOwner>
{
    public Dictionary<Type, Func<object, string?>> TypeSerializers { get; } = new();
    public Dictionary<string, Func<object, string>> PropertySerializers { get; } = new();
    public HashSet<Type> ExcludedTypes { get; } = [];
    public HashSet<string> ExcludedProperties { get; } = [];
    
    public readonly HashSet<object> Visited = [];
    public Dictionary<string, int> StringTrimmingRules { get; } = new();

    public PrintingConfig<TOwner> Excluding<TProp>()
    {
        ExcludedTypes.Add(typeof(TProp));
        return this;
    }
    public PrintingConfig<TOwner> Excluding(Expression<Func<TOwner, object>> selector)
    {
        ExcludedProperties.Add(GetPropertyName(selector));
        return this;
    }

    public TypePrintingConfig<TOwner, TProp> Printing<TProp>()
    {
        return new TypePrintingConfig<TOwner, TProp>(this);
    }

    public PropertyPrintingConfig<TOwner, TProp> Printing<TProp>(
        Expression<Func<TOwner, TProp>> selector)
    {
        return new PropertyPrintingConfig<TOwner, TProp>(this, selector);
    }

    internal static string GetPropertyName<TProp>(Expression<Func<TOwner, TProp>> selector)
    {
        return selector.Body switch
        {
            MemberExpression m => m.Member.Name,
            UnaryExpression { Operand: MemberExpression m2 } => m2.Member.Name,
            _ => throw new ArgumentException("Expression must be a property")
        };
    }

    public string PrintToString(TOwner obj)
    {
        var sb = new StringBuilder();
        Visited.Clear();
        if (obj != null)
            obj.Print(this, sb, 0);
        return sb.ToString();
    }
}