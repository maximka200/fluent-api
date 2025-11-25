using System.Globalization;
using FluentAssertions;
using ObjectPrinting;
using ObjectPrinting.Extensions;

namespace ObjectPrinterTests
{
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
        public void ShouldExcludeType_FromSerialization()
        {
            var obj = new A { Id = Guid.NewGuid(), Name = "Alex" };
            var printer = ObjectPrinter.For<A>()
                .Excluding<Guid>();

            var result = printer.PrintToString(obj);

            result.Should().NotContain(nameof(A.Id));
        }
        
        [Test]
        public void ShouldUseCustomSerialization_ForType()
        {
            var obj = new A { Number = 42 };
            var printer = ObjectPrinter.For<A>()
                .Printing<int>()
                .Using(x => $"INT({x})");

            var result = printer.PrintToString(obj);

            result.Should().Contain("INT(42)");
        }
        
        [Test]
        public void ShouldApplyCultureInfo_ToType()
        {
            var obj = new A { Price = 1234.56 };
            var printer = ObjectPrinter.For<A>()
                .Printing<double>()
                .Using(CultureInfo.GetCultureInfo("fr-FR"));

            var result = printer.PrintToString(obj);

            result.Should().Contain("1234,56");
        }
        
        [Test]
        public void ShouldApplyСustomSerialization_ToProperty()
        {
            var obj = new A { Name = "Alex" };
            var printer = ObjectPrinter.For<A>()
                .Printing(a => a.Name)
                .Using(n => $"NAME={n}");

            var result = printer.PrintToString(obj);

            result.Should().Contain("NAME=Alex");
        }
        
        [Test]
        public void ShouldTrimString_Property()
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
        public void ShouldExcludeSpecific_Property()
        {
            var obj = new A { Name = "Alex", Number = 10 };
            var printer = ObjectPrinter.For<A>()
                .Excluding(a => a.Number);
            
            var result = printer.PrintToString(obj);

            result.Should().NotContain(nameof(A.Number));
        }
        
        [Test]
        public void ShouldHandleCyclicReferences_InProperty()
        {
            var parent = new B();
            var child = new B { Parent = parent };
            parent.Data = new A { Name = "Child" };
            parent.Parent = child;
            var printer = ObjectPrinter.For<B>();

            var act = () => printer.PrintToString(parent);
            var result = printer.PrintToString(parent);
            
            act.Should().NotThrow();
            result.Should().Contain("<cyclic reference>");
        }
    }
}