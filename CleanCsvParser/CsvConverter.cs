using System.Reflection;
using System.Linq.Expressions;

namespace CleanCsvParser;

internal static class CsvConverter
{
    private static readonly Dictionary<Type, object> _setterCache = new();

    public static IEnumerable<T> ConvertTo<T>(IEnumerable<string[]> rows, string[]? headers = null) where T : new()
    {
        var setters = GetSetters<T>(headers);

        foreach (var row in rows)
        {
            var obj = new T();

            for (int i = 0; i < row.Length; i++)
            {
                if (i < setters.Length && setters[i] != null)
                    setters[i]!(obj, row[i]);
            }

            yield return obj;
        }
    }

    private static Action<T, string>?[] GetSetters<T>(string[]? headers) where T : new()
    {
        var type = typeof(T);

        if (_setterCache.TryGetValue(type, out var cached) && cached is Action<T, string>?[] arr)
            return arr;

        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite)
                        .ToArray();

        Action<T, string>?[] setters;

        if (headers != null)
        {
            setters = headers.Select(h =>
            {
                var prop = props.FirstOrDefault(p => p.Name.Equals(h, StringComparison.OrdinalIgnoreCase));
                return prop != null ? CreateSetter<T>(prop) : null;
            }).ToArray();
        }
        else
        {
            setters = props.Select(CreateSetter<T>).ToArray();
        }

        _setterCache[type] = setters;
        return setters;
    }

    private static Action<T, string>? CreateSetter<T>(PropertyInfo prop)
    {
        var objParam = Expression.Parameter(typeof(T), "obj");
        var valueParam = Expression.Parameter(typeof(string), "val");

        var convertMethod = typeof(CsvConverter).GetMethod(nameof(ConvertValue), BindingFlags.NonPublic | BindingFlags.Static)!
                                                 .MakeGenericMethod(prop.PropertyType);

        var convertExpr = Expression.Call(convertMethod, valueParam);
        var assign = Expression.Assign(Expression.Property(objParam, prop), convertExpr);

        return Expression.Lambda<Action<T, string>>(assign, objParam, valueParam).Compile();
    }

    private static TVal ConvertValue<TVal>(string input)
    {
        return (TVal)Convert.ChangeType(input, typeof(TVal));
    }
}
