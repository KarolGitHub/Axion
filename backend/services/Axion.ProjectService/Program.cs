var builder = WebApplication.CreateBuilder(args);

// Observability: OpenTelemetry Tracing & Metrics
var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(serviceName: "Axion.ProjectService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint))
        .AddConsoleExporter())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint))
        .AddConsoleExporter());

// Add services to the container.

// CORS for CDN origin
var cdnOrigin = builder.Configuration["CDN_ORIGIN"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("CdnCors", policy =>
    {
        if (!string.IsNullOrWhiteSpace(cdnOrigin))
        {
            policy.WithOrigins(cdnOrigin)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddControllers();
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

app.UseCors("CdnCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
