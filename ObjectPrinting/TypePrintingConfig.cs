using System;

namespace ObjectPrinting;

public class TypePrintingConfig<TOwner, TProp>(PrintingConfig<TOwner> parent)
{
    public PrintingConfig<TOwner> ParentConfig { get; } = parent;

    public PrintingConfig<TOwner> Using(Func<TProp, string> serializer)
    {
        ParentConfig.TypeSerializers[typeof(TProp)] = x => serializer((TProp)x);
        return ParentConfig;
    }

    public PrintingConfig<TOwner> Using(IFormatProvider provider)
    {
        ParentConfig.TypeSerializers[typeof(TProp)] = x => 
            Convert.ToString(x, provider);

        return ParentConfig;
    }
}