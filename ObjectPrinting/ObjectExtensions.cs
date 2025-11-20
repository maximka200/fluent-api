using System.Collections;
using System.Text;

namespace ObjectPrinting;

public static class ObjectExtensions
{
    public static void Print<TOwner>(this object? obj, PrintingConfig<TOwner> config,
        StringBuilder sb, int indent)
    {
        if (obj is null)
        {
            sb.Append("null");
            return;
        }

        if (!config.Visited.Add(obj))
        {
            sb.Append("<cyclic reference>");
            return;
        }

        var type = obj.GetType();
        
        if (config.ExcludedTypes.Contains(type))
            return;
        
        if (config.TypeSerializers.TryGetValue(type, out var typeSer))
        {
            sb.Append(typeSer(obj));
            return;
        }
        
        if (obj is IDictionary dict)
        {
            dict.PrintDictionary(config, sb, indent);
            return;
        }
        
        if (obj is IEnumerable enumerable && obj is not string)
        {
            enumerable.PrintEnumerable(config, sb, indent);
            return;
        }
        
        if (type.IsPrimitive || obj is string || type.GetProperties().Length == 0)
        {
            sb.Append(obj);
            return;
        }
        
        PrintDefaultObject(obj, config, sb, indent);
    }
    
    private static void PrintDefaultObject<TOwner>(object obj, PrintingConfig<TOwner> config,
        StringBuilder sb, int indent)
    {
        var type = obj.GetType();

        sb.AppendLine(type.Name + ":");

        foreach (var prop in type.GetProperties())
        {
            var name = prop.Name;
            var propType = prop.PropertyType;

            if (config.ExcludedProperties.Contains(name) ||
                config.ExcludedTypes.Contains(propType))
                continue;

            var value = prop.GetValue(obj);

            sb.Append(new string('\t', indent + 1));
            sb.Append(name);
            sb.Append(" = ");

            if (config.PropertySerializers.TryGetValue(name, out var propSer))
            {
                sb.AppendLine(value != null ? propSer(value) : "null");
                continue;
            }

            if (value is string s &&
                config.StringTrimmingRules.TryGetValue(name, out var max))
            {
                sb.AppendLine(s.Length <= max ? s : s[..max]);
                continue;
            }

            if (value is null)
            {
                sb.AppendLine("null");
                continue;
            }

            value.Print(config, sb, indent + 1);
            sb.AppendLine();
        }
    }
}
