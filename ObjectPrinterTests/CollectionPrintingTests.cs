using FluentAssertions;
using ObjectPrinting;

namespace ObjectPrinterTests;

[TestFixture]
public class CollectionPrintingSnapshotTests
{
    private class Address
    {
        public string City { get; set; } = "";
    }

    private class Person
    {
        public string Name { get; set; } = "";
        public int[] Scores { get; set; } = [];
        public List<string> Tags { get; set; } = [];
        public Dictionary<string, Address> Addresses { get; set; } = new();
    }

    private class Team
    {
        public string Title { get; set; } = "";
        public List<Person> Members { get; set; } = new();
    }

    [Test]
    public void IntArray_Snapshot()
    {
        var config = new PrintingConfig<int[]>();
        var value = new[] { 1, 2, 3 };
        
        var actual = config.PrintToString(value);
        var expected = Helper.Read("IntArray.txt");
        
        actual.Should().Be(expected);
    }

    [Test]
    public void StringList_Snapshot()
    {
        var config = new PrintingConfig<List<string>>();
        var value = new List<string> { "one", "two" };

        var actual = config.PrintToString(value);
        var expected = Helper.Read("StringList.txt");

        actual.Should().Be(expected);
    }

    [Test]
    public void Dictionary_Primitive_Snapshot()
    {
        var config = new PrintingConfig<Dictionary<string, int>>();
        var value = new Dictionary<string, int>
        {
            ["a"] = 1,
            ["b"] = 2
        };

        var actual = config.PrintToString(value);
        var expected = Helper.Read("Dictionary_Primitive.txt");

        actual.Should().Be(expected);
    }

    [Test]
    public void PersonWithCollections_Snapshot()
    {
        var config = new PrintingConfig<Person>();
        var person = new Person
        {
            Name = "John",
            Scores = [10, 20],
            Tags = ["dev", "qa"],
            Addresses = new Dictionary<string, Address>
            {
                ["home"] = new() { City = "Chelyabinsk" },
                ["work"] = new() { City = "Sverdlovsk" }
            }
        };

        var actual = config.PrintToString(person);
        var expected = Helper.Read("Person_With_Collections.txt");

        actual.Should().Be(expected);
    }

    [Test]
    public void List_Of_Lists_Snapshot()
    {
        var config = new PrintingConfig<List<List<int>>>();
        var value = new List<List<int>>
        {
            new() { 1, 2 },
            new() { 3 }
        };

        var actual = config.PrintToString(value);
        var expected = Helper.Read("List_Of_Lists.txt");

        actual.Should().Be(expected);
    }

    [Test]
    public void DictionaryWithListValues_Snapshot()
    {
        var config = new PrintingConfig<Dictionary<string, List<int>>>();
        var value = new Dictionary<string, List<int>>
        {
            ["first"] = [1, 2],
            ["second"] = [3]
        };

        var actual = config.PrintToString(value);
        var expected = Helper.Read("Dictionary_With_ListValues.txt");

        actual.Should().Be(expected);
    }

    [Test]
    public void TeamWithMembers_Snapshot()
    {
        var config = new PrintingConfig<Team>();
        var team = new Team
        {
            Title = "My_Command",
            Members =
            {
                new Person
                {
                    Name = "Max",
                    Scores = [1],
                    Tags = ["dev"]
                },
                new Person
                {
                    Name = "Kate",
                    Scores = [2, 3],
                    Tags = ["qa"]
                }
            }
        };

        var actual = config.PrintToString(team);
        var expected = Helper.Read("Team_With_Members.txt");

        actual.Should().Be(expected);
    }
}
