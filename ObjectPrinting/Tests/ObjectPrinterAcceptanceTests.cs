using System;
using System.Globalization;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person {Id = new Guid(), Name = "Alex", Age = 19 };
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>().Using(i => i.ToString("X"))
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                .Printing(p => p.Name).TrimmedToLength(3)
                .Excluding(p => p.Age);
            
            var s1 = printer.PrintToString(person);
            
            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию  
            var s2 = printer.PrintToString(person);
            
            //8. ...с конфигурированием
            
            Console.WriteLine(s1);
        }
    }
}