using Asp.Versioning;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.transaction.api.Configuration;
using ssptb.pe.tdlt.transaction.api.Configuration.Security;
using ssptb.pe.tdlt.transaction.common.Secrets;
using ssptb.pe.tdlt.transaction.common.Settings;
using ssptb.pe.tdlt.transaction.data.HealthCheck;
using ssptb.pe.tdlt.transaction.dto.Mapster;
using ssptb.pe.tdlt.transaction.secretsmanager.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ssptb.pe.tdlt.transaction.api;

public static class ProgramExtesions
{
    public static IServiceCollection AddCustomMvc(this IServiceCollection services, WebApplicationBuilder builder)
    {
        string[] domains = builder.Configuration.GetSection("CorsDomains").Get<string[]>();

        services.AddControllersWithViews();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .WithOrigins(domains)
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        services.AddOptions();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }

    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
        builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
        builder.Services.Configure<SecretManagerSettings>(builder.Configuration.GetSection("SecretManagerSettings"));

        return services;
    }

    public static IServiceCollection AddSecretsConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        var secretManagerService = (ISecretManagerService)serviceProvider.GetService(typeof(ISecretManagerService));

        CouchBaseSecrets secretsPostgres = secretManagerService.GetCouchBaseSecrets().GetAwaiter().GetResult();

        services.Configure<CouchBaseSettings>(options =>
        {
            options.ConnectionString = secretsPostgres.ConnectionString;
            options.BucketName = secretsPostgres.BucketName;
            options.UserName = secretsPostgres.UserName;
            options.Password = secretsPostgres.Password;
        });

        return services;
    }

    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig mapsterConfiguration = MapsterConfiguration.Configuration();

        services.AddSingleton(mapsterConfiguration);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        return services;
    }

    public static WebApplication ConfigurationSwagger(this WebApplication app)
    {
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();

            foreach (var description in descriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }

        });

        return app;
    }

    public static IServiceCollection AddDatabaseHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<CouchBaseHealthCheck>("CouchBase");

        return services;
    }

    public static WebApplication AddSecurityHeaders(this WebApplication app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>(new SecurityHeadersBuilder().AddDefaultSecurePolicy().Build());
        return app;
    }

    public static IServiceCollection AddAntiForgeryToken(this IServiceCollection services)
    {
        services.AddSingleton<IAntiforgeryAdditionalDataProvider, CustomAntiforgeryDataProvider>();

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        return services;
    }

}
