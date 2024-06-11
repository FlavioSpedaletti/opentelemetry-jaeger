using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using opentelemetry_jaeger_api.Configuration;
using opentelemetry_jaeger_api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("myService-api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(o =>
            {
                // to trace only api requests
                //o.Filter = (context) => !string.IsNullOrEmpty(context.Request.Path.Value) && context.Request.Path.Value.Contains("Api", StringComparison.InvariantCulture);

                // example: only collect telemetry about HTTP GET requests
                // return httpContext.Request.Method.Equals("GET");

                // enrich activity with http request and response
                o.EnrichWithHttpRequest = (activity, httpRequest) => { activity.SetTag("requestProtocol", httpRequest.Protocol); };
                o.EnrichWithHttpResponse = (activity, httpResponse) => { activity.SetTag("responseLength", httpResponse.ContentLength); };

                // automatically sets Activity Status to Error if an unhandled exception is thrown
                o.RecordException = true;
                o.EnrichWithException = (activity, exception) =>
                {
                    activity.SetTag("exceptionType", exception.GetType().ToString());
                    activity.SetTag("stackTrace", exception.StackTrace);
                };
            })
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(opt =>
            {
                opt.SetDbStatementForText = true;
                opt.SetDbStatementForStoredProcedure = true;
                opt.EnrichWithIDbCommand = (activity, command) =>
                {
                    // var stateDisplayName = $"{command.CommandType} main";
                    // activity.DisplayName = stateDisplayName;
                    // activity.SetTag("db.name", stateDisplayName);
                };
            })
            .AddRedisInstrumentation();
        //.AddNpgsql();

        tracing.AddOtlpExporter();
    });

builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseInMemoryDatabase("cars"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.MapGroup("v1/cars")
    .MapEndpoints()
    .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
