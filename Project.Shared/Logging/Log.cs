namespace Project.Shared;

/// <summary>
/// Lightweight replacement for Serilog's static Log API.
/// </summary>
public static class Log
{
    public static void Error(Exception ex, string messageTemplate, params object?[] propertyValues)
        => Console.Error.WriteLine($"[ERR] {Format(messageTemplate, propertyValues)}{Environment.NewLine}{ex}");

    public static void Error(string messageTemplate, Exception ex)
        => Console.Error.WriteLine($"[ERR] {messageTemplate}{Environment.NewLine}{ex}");

    public static void Error(string message)
        => Console.Error.WriteLine($"[ERR] {message}");

    public static void Warning(string message)
        => Console.WriteLine($"[WRN] {message}");

    public static void Information(string message)
        => Console.WriteLine($"[INF] {message}");

    private static string Format(string messageTemplate, object?[] propertyValues)
        => propertyValues is { Length: > 0 }
            ? $"{messageTemplate} ({string.Join(", ", propertyValues)})"
            : messageTemplate;
}
