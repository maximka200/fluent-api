using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting;

public class PrintingConfig<TOwner>
{
    internal Dictionary<Type, Func<object, string?>> TypeSerializers { get; } = new();
    internal Dictionary<string, Func<object, string>> PropertySerializers { get; } = new();
    private HashSet<Type> ExcludedTypes { get; } = [];
    private HashSet<string> ExcludedProperties { get; } = [];
    
    private readonly HashSet<object> visited = [];
    internal Dictionary<string, int> StringTrimmingRules { get; } = new();

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
        if (obj != null) Print(obj, sb, 0);
        return sb.ToString();
    }

    private void Print(object obj, StringBuilder sb, int indent)
    {
        var type = obj.GetType();
        
        if (obj == null)
        {
            sb.Append("null");
            return;
        }

        if (visited.Contains(obj))
        {
            sb.Append("<cyclic reference>");
            return;
        }
        
        visited.Add(obj);
        
        if (ExcludedTypes.Contains(type))
            return;
        
        if (TypeSerializers.TryGetValue(type, out var typeSer))
        {
            sb.Append(typeSer(obj));
            return;
        }
        
        if (type.IsPrimitive || obj is string || type.GetProperties().Length == 0)
        {
            sb.Append(obj);
            return;
        }

        sb.AppendLine(type.Name + ":");

        foreach (var prop in type.GetProperties())
        {
            var name = prop.Name;
            var propType = prop.PropertyType;
            
            if (ExcludedProperties.Contains(name) || ExcludedTypes.Contains(propType))
                continue;

            var value = prop.GetValue(obj);

            sb.Append(new string('\t', indent + 1));
            sb.Append(name + " = ");
            
            if (PropertySerializers.TryGetValue(name, out var propSer))
            {
                if (value != null) sb.AppendLine(propSer(value));
                continue;
            }
            
            if (value is string s &&
                StringTrimmingRules.TryGetValue(name, out var max))
            {
                sb.AppendLine(s.Length <= max ? s : s[..max]);
                continue;
            }

            if (value != null) Print(value, sb, indent + 1);
            sb.AppendLine();
        }
    }
}