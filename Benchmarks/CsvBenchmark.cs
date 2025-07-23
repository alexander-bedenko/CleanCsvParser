using BenchmarkDotNet.Attributes;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvParser.Benchmarks;

[MemoryDiagnoser]
public class CsvBenchmark
{
    private string csvData = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Name,Age,City");
        for (int i = 0; i < 10_000; i++)
        {
            sb.AppendLine($"Name{i}, {20 + i % 50}, City{i % 100}");
        }

        csvData = sb.ToString();
    }

    [Benchmark(Baseline = true)]
    public List<Person> CleanCsvParser_Parse()
    {
        return CleanCsvParser.CsvParser.Parse<Person>(csvData).ToList();
    }

    [Benchmark]
    public List<Person> CsvHelper_Parse()
    {
        using var reader = new StringReader(csvData);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<Person>().ToList();
    }
}


public class Person
{
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string City { get; set; } = default!;
}
