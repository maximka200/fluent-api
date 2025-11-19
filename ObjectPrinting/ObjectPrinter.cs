namespace ObjectPrinting;

public static class ObjectPrinter
{
    public static PrintingConfig<T> For<T>()
    {
        return new PrintingConfig<T>();
    }
    
    public static string PrintToString<TOwner>(object obj)
    {
        var printer = new PrintingConfig<TOwner>();
        return printer.PrintToString((TOwner)obj);
    }
}
