using System.Collections;
using System.Text;

namespace ObjectPrinting;

public static class CollectionPrintingExtensions
{
    public static void PrintDictionary<TOwner>(this IDictionary dict, PrintingConfig<TOwner> config, 
        StringBuilder sb, int indent)
    {
        sb.AppendLine(dict.GetType().Name + " {");
        foreach (DictionaryEntry entry in dict)
        {
            sb.Append(new string('\t', indent + 1));
            sb.Append('[');
            entry.Key.Print(config, sb, indent + 1);
            sb.Append("] = ");

            if (entry.Value is null)
            {
                sb.AppendLine("null");
            }
            else
            {
                entry.Value.Print(config, sb, indent + 1);
                sb.AppendLine();
            }
        }

        sb.Append(new string('\t', indent));
        sb.Append('}');
    }

    public static void PrintEnumerable<TOwner>(this IEnumerable enumerable, PrintingConfig<TOwner> config,
        StringBuilder sb, int indent)
    {
        sb.AppendLine(enumerable.GetType().Name + " [");
        foreach (var item in enumerable)
        {
            sb.Append(new string('\t', indent + 1));
            if (item is null)
            {
                sb.AppendLine("null");
            }
            else
            {
                item.Print(config, sb, indent + 1);
                sb.AppendLine();
            }
        }

        sb.Append(new string('\t', indent));
        sb.Append(']');
    }
}