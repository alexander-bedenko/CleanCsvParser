using CleanCsvParser.Models;

namespace CleanCsvParser;

internal static class CsvReader
{
    public static (string[]? headers, IEnumerable<string[]> rows) Read(TextReader reader, CsvOptions options)
    {
        var lines = CsvTokenizer.TokenizeLines(reader, options).ToList();

        if (options.HasHeader && lines.Count > 0)
        {
            var headers = lines[0];
            var data = lines.Skip(1);
            return (headers, data);
        }

        return (null, lines);
    }

    public static async IAsyncEnumerable<string[]> ReadAsync(TextReader reader, CsvOptions options)
    {
        string? line;
        bool isFirst = true;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (options.SkipEmptyLines && string.IsNullOrWhiteSpace(line))
                continue;

            if (isFirst && options.HasHeader)
            {
                isFirst = false;
                continue; // skip header
            }

            yield return CsvTokenizer.TokenizeLine(line.AsSpan(), options.Delimiter).ToArray();
        }
    }
}