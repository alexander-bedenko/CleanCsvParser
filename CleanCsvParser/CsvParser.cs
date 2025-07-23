using CleanCsvParser.Models;

namespace CleanCsvParser;

public static class CsvParser
{
    public static IEnumerable<T> Parse<T>(string csv, CsvOptions? options = null) where T : new()
    {
        options ??= new CsvOptions();
        using var reader = new StringReader(csv);
        var (headers, rows) = CsvReader.Read(reader, options);
        return CsvConverter.ConvertTo<T>(rows, headers);
    }

    public static async IAsyncEnumerable<T> ParseAsync<T>(TextReader reader, CsvOptions? options = null) where T : new()
    {
        options ??= new CsvOptions();

        string[]? headers = null;
        if (options.HasHeader)
        {
            var headerLine = await reader.ReadLineAsync();
            if (headerLine == null)
                yield break;

            headers = CsvTokenizer.TokenizeLine(headerLine.AsSpan(), options.Delimiter).ToArray();
        }

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var tokens = CsvTokenizer.TokenizeLine(line.AsSpan(), options.Delimiter).ToArray();

            var list = CsvConverter.ConvertTo<T>(new[] { tokens }, headers);
            T item = list.First();

            yield return item;
        }
    }
}
