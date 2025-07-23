using CleanCsvParser;
using CleanCsvParser.Models;

public class Person
{
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string City { get; set; } = default!;
}

public class CsvParserTests
{
    private const string CsvWithHeader = """
Name,Age,City
"John",30,"WildWood"
"Mark",25,"NY"
""";

    private const string CsvWithoutHeader = """
"John",30,"WildWood"
"Mark",25,"NY"
""";

    [Fact]
    public void Parse_WithHeader_ReturnsCorrectRecords()
    {
        var people = CsvParser.Parse<Person>(CsvWithHeader).ToList();

        Assert.Equal(2, people.Count);
        Assert.Equal("John", people[0].Name);
        Assert.Equal(30, people[0].Age);
        Assert.Equal("WildWood", people[0].City);
    }

    [Fact]
    public void Parse_WithoutHeader_UsesPropertyOrder()
    {
        var options = new CsvOptions { HasHeader = false };
        var people = CsvParser.Parse<Person>(CsvWithoutHeader, options).ToList();

        Assert.Equal(2, people.Count);
        Assert.Equal("John", people[0].Name);
    }

    [Fact]
    public void Parse_SkipsEmptyLines()
    {
        var csv = """
            Name,Age,City
            "John",30,"WildWood"

            "Mark",25,"NY"
            """;
        var options = new CsvOptions { SkipEmptyLines = true };
        var people = CsvParser.Parse<Person>(csv, options).ToList();

        Assert.Equal(2, people.Count);
    }

    [Fact]
    public async Task ParseAsync_WorksCorrectly()
    {
        using var reader = new StringReader(CsvWithHeader);
        var results = new List<Person>();

        await foreach (var p in CsvParser.ParseAsync<Person>(reader))
            results.Add(p);

        Assert.Equal(2, results.Count);
        Assert.Equal("Mark", results[1].Name);
    }

    [Fact]
    public void TokenizeLine_ParsesQuotedFields()
    {
        var line = "\"John, Cobain\",30,\"WildWood\"";
        var tokens = CsvTokenizer.TokenizeLine(line.AsSpan(), ',').ToArray();

        Assert.Equal(3, tokens.Length);
        Assert.Equal("John, Cobain", tokens[0]);
    }

    [Fact]
    public void TokenizeLine_HandlesEscapedQuotes()
    {
        var line = @"""John """"Kurt"""" Cobain"",30,WildWood";
        var tokens = CsvTokenizer.TokenizeLine(line.AsSpan(), ',').ToArray();

        Assert.Equal("John \"Kurt\" Cobain", tokens[0]);
    }
}
