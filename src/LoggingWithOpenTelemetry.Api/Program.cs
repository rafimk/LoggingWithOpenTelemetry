using LoggingWithOpenTelemetry.Api.Services;
using LoggingWithOpenTelemetry.Api.Tracing;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);


builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddDebug()
    .AddOpenTelemetry(options =>
    {
        options.AddConsoleExporter()
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
            .AddService("Logging.Net"))
        .AddProcessor(new ActivityEventLogProcessor())
        .IncludeScopes = true;
    });

builder.Services.AddOpenTelemetry()
    .WithTracing(builder =>
        builder
            .AddSource("Tracing.Net")
            .AddAspNetCoreInstrumentation()
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("Tracing.Net"))
    .AddConsoleExporter()
    .AddJaegerExporter());

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (IUserService userService, ILogger<Program> logger) =>
{
    logger.LogDebug($"This is a {LogLevel.Debug} message");
    logger.LogInformation($"{LogLevel.Information} message are used to provide contextual information");
    logger.LogError(new Exception("Application exception"), "There are usually accompanied by an exception");
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    userService.Login("codemaze", "P@ssw0rd");

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
