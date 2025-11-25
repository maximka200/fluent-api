using System;
using System.Text;
using ObjectPrinting.Extensions;

namespace ObjectPrinting.Strategies;

public class DefaultObjectStrategy : IPrintStrategy
{
    public bool CanPrint<TOwner>(object obj, Type type, PrintingConfig<TOwner> config)
        => true; 

    public void Print<TOwner>(object obj, Type type, PrintingConfig<TOwner> config,
        StringBuilder sb, int indent)
    {
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