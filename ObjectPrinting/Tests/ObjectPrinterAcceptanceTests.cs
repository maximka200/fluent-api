using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person {Id = new Guid(), Name = "Alex", Age = 112, Height = 1.89,
                Child = new Person {Id = new Guid(), Name = "Roman", Age = 5, Height = 1.0}
            };
            
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>().Using(x => (x - 1).ToString())
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                .Printing(p => p.Name).TrimmedToLength(3)
                .Excluding(p => p.Id)
                .Excluding<Guid>();
            
            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию  
            var s2 = ObjectPrinter.PrintToString<Person>(person);
            //8. ...с конфигурированием
            var s1 = printer.PrintToString(person);
            
            Console.WriteLine(s1);
            Console.WriteLine("-----");
            Console.WriteLine(s2);
        }
    }
    [TestFixture]
    public class ObjectPrinterTests
    {
        private class A
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public double Price { get; set; }
            public Guid Id { get; set; }
        }

        private class B
        {
            public A Data { get; set; }
            public B Parent { get; set; } 
        }
        
        [Test]
        public void Should_Exclude_Type_From_Serialization()
        {
            var obj = new A { Id = Guid.NewGuid(), Name = "Alex" };

            var printer = ObjectPrinter.For<A>()
                .Excluding<Guid>();

            var result = printer.PrintToString(obj);

            result.Should().NotContain(nameof(A.Id));
        }
        
        [Test]
        public void Should_Use_Custom_Serialization_For_Type()
        {
            var obj = new A { Number = 42 };

            var printer = ObjectPrinter.For<A>()
                .Printing<int>()
                .Using(x => $"INT({x})");

            var result = printer.PrintToString(obj);

            result.Should().Contain("INT(42)");
        }
        
        [Test]
        public void Should_Apply_CultureInfo_To_Type()
        {
            var obj = new A { Price = 1234.56 };

            var printer = ObjectPrinter.For<A>()
                .Printing<double>()
                .Using(CultureInfo.GetCultureInfo("fr-FR")); // 1234,56

            var result = printer.PrintToString(obj);

            result.Should().Contain("1234,56");
        }
        
        [Test]
        public void Should_Apply_Custom_Serialization_To_Specific_Property()
        {
            var obj = new A { Name = "Alex" };

            var printer = ObjectPrinter.For<A>()
                .Printing(a => a.Name)
                .Using(n => $"NAME={n}");

            var result = printer.PrintToString(obj);

            result.Should().Contain("NAME=Alex");
        }
        
        [Test]
        public void Should_Trim_String_Property()
        {
            var obj = new A { Name = "Alexander" };

            var printer = ObjectPrinter.For<A>()
                .Printing(a => a.Name)
                .TrimmedToLength(4);

            var result = printer.PrintToString(obj);

            result.Should().Contain("Alex");
            result.Should().NotContain("Alexander");
        }
        
        [Test]
        public void Should_Exclude_Specific_Property()
        {
            var obj = new A { Name = "Alex", Number = 10 };

            var printer = ObjectPrinter.For<A>()
                .Excluding(a => a.Number);

            var result = printer.PrintToString(obj);

            result.Should().NotContain(nameof(A.Number));
        }
        
        [Test]
        public void Should_Handle_Cyclic_References()
        {
            var parent = new B();
            var child = new B { Parent = parent };
            parent.Data = new A { Name = "Child" };
            parent.Parent = child;

            var printer = ObjectPrinter.For<B>();

            Action act = () => printer.PrintToString(parent);

            act.Should().NotThrow();

            var result = printer.PrintToString(parent);

            result.Should().Contain("<cyclic reference>");
        }
    }
}