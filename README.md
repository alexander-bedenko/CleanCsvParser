# CleanCsvParser

⚡️ A fast, safe, and minimal CSV parser for .NET — no dependencies, low allocations, high performance.

## Features

- ✅ Handles quoted fields, escaping, and empty lines
- 🚀 Uses `Span<char>` and `TextReader` for minimal allocations
- 🧱 Converts rows to POCOs using compiled expressions (no runtime reflection)
- 💡 Suitable for ASP.NET, Blazor, WPF, Telegram bots, console apps, and more

## Installation

Coming soon to [NuGet](https://www.nuget.org/).

```bash
dotnet add package CleanCsvParser

# Quick Start
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

string csv = "Name,Age\nAlice,30\nBob,25";

var people = CsvParser.Parse<Person>(csv);

foreach (var person in people)
{
    Console.WriteLine($"{person.Name} is {person.Age} years old.");
}

# Configuration
Customize behavior using CsvOptions:

var options = new CsvOptions
{
    Delimiter = ';',
    HasHeader = true,
    SkipEmptyLines = true
};

Goals
🔬 High performance (no LINQ overhead)

🧪 Easy to test and benchmark

🔒 Safe parsing without exceptions in common scenarios

📦 Small and dependency-free
