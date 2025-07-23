namespace CleanCsvParser.Models
{
    public class CsvOptions
    {
        public char Delimiter { get; init; } = ',';
        public bool HasHeader { get; init; } = true;
        public bool SkipEmptyLines { get; init; } = true;
    }
}
