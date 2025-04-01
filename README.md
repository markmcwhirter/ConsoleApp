
public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();

        var logEntry = new LogEntry
        {
            LogLevel = logLevel,
            Category = _categoryName,
            Message = message,
            Exception = exception?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        dbContext.LogEntries.Add(logEntry);
        dbContext.SaveChanges(); // Consider making this async in high-throughput apps
    }
}
------------------------
public class DatabaseLoggerProvider : ILoggerProvider
{
    private readonly Func<LogLevel, bool> _filter;
    private readonly IServiceProvider _serviceProvider;

    public DatabaseLoggerProvider(Func<LogLevel, bool> filter, IServiceProvider serviceProvider)
    {
        _filter = filter;
        _serviceProvider = serviceProvider;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger(categoryName, _filter, _serviceProvider);
    }

    public void Dispose() { }
}
-------------------------
public static class DatabaseLoggerExtensions
{
    public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder, Func<LogLevel, bool> filter)
    {
        builder.Services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>(sp =>
            new DatabaseLoggerProvider(filter, sp));
        return builder;
    }
}

-------------------------------------
var builder = WebApplication.CreateBuilder(args);

// Register LoggingDbContext (use your actual DB connection string)
builder.Services.AddDbContext<LoggingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoggingDb")));

// Add logging provider
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // optional
builder.Logging.AddDatabaseLogger(logLevel => logLevel >= LogLevel.Information);

var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("This is a test log to DB at {time}", DateTime.Now);
    return "Hello World!";
});

app.Run();
