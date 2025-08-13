using API.Page.DependencyInjection;
using API.Paging.DependencyInjection;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using XApi.API.Autocomplete.Endpoints;
using XApi.API.Categories.DependencyInjection;
using XApi.API.DependencyInjection;
using XApi.API.Linkbox.DependencyInjection;
using XApi.API.Page.Endpoints;
using XApi.API.Pornstars.DependencyInjection;
using XApi.API.Pornstars.Endpoints;
using XApi.API.Search.DependencyInjection;
using XApi.API.Search.Endpoints;
using XApi.API.Suggestion.DependencyInjection;
using XApi.API.Tags.DependencyInjection;
using XApi.API.Tags.Endpoints;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://loki:3100", labels: new List<LokiLabel>
    {
        new LokiLabel { Key = "app", Value = "x-api" },
        new LokiLabel { Key = "env", Value = "production" }
    })
    .CreateLogger();

try
{
    Log.Information("Starting application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors(options =>
    {
        // TODO : be more strict
        options.AddPolicy("AllowAll",
            policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Description = "Clé API pour l'autorisation. Utilisez le format 'Bearer {votre-clé-api}'.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKey"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });

    #region Custom dependencies DI
    builder.Services.AddMapster();
    builder.Services.AddMemoryCache();
    builder.Services.AddTagsDependencies();
    builder.Services.AddCategoriesDependencies();
    builder.Services.AddPornstarsDependencies();
    builder.Services.AddSearchDependencies();
    builder.Services.AddSeoDependencies();
    builder.Services.AddPagingsDependencies();
    builder.Services.AddPagesDependencies();
    builder.Services.AddLinkboxesDependencies();
    builder.Services.AddVideoDependencies();
    builder.Services.AddSuggestionDependencies();
    #endregion

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("AllowAll");
    app.UseHttpsRedirection();

    // Prometheus metrics middleware
    app.UseMetricServer(); // Exposes /metrics endpoint
    app.UseHttpMetrics(); // Adds HTTP request metrics automatically

    #region Custom endpoints registration
    //app.MapTagEndpoints();
    //app.MapPornstarEndpoints();
    //app.MapSearchEndpoints();
    app.MapPageEndpoints();
    app.MapAutocompleteEndpoints();

    #endregion

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
