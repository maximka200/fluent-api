namespace ObjectPrinterTests;

public static class Helper
{
    public static string Read(string fileName)
    {
        var testDir = TestContext.CurrentContext.TestDirectory;
        
        var dir = Path.Combine(testDir, "ExpectedResults"); 

        var path = Path.Combine(dir, fileName);
        if (!File.Exists(path))
            Assert.Fail($"File not found: {path}");

        return File.ReadAllText(path);
    }
}