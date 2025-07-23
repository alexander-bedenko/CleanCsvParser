using CleanCsvParser.Models;

namespace CleanCsvParser;

internal static class CsvTokenizer
{
    public static IEnumerable<string[]> TokenizeLines(TextReader reader, CsvOptions options)
    {
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (options.SkipEmptyLines && string.IsNullOrWhiteSpace(line))
                continue;

            yield return TokenizeLine(line.AsSpan(), options.Delimiter).ToArray();
        }
    }

    public static IEnumerable<string> TokenizeLine(ReadOnlySpan<char> line, char delimiter)
    {
        var result = new List<string>();
        Span<char> buffer = stackalloc char[1024];
        var sb = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        result.Add(sb.ToString());
        return result;
    }
}