using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using QuickOrder.Api.Middleware;
using QuickOrder.Application.Behaviors;
using QuickOrder.Application.Features.Menu.Queries;
using QuickOrder.Application.Features.Orders.Validators;
using QuickOrder.Infrastructure;
using QuickOrder.Infrastructure.Hubs;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext());

    builder.Services.AddControllers(opts =>
        opts.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseTransformer())));
    builder.Services.AddRazorPages();
    builder.Services.AddOpenApi();
    builder.Services.AddSignalR();

    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(GetMenuQuery).Assembly);
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    });

    builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddHealthChecks()
        .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(policy =>
        {
            if (allowedOrigins.Contains("*"))
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            else if (allowedOrigins.Length > 0)
                policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            else
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }));

    var rateLimitPermit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 100);
    var rateLimitWindow = builder.Configuration.GetValue<int>("RateLimiting:WindowSeconds", 60);
    builder.Services.AddRateLimiter(opts =>
    {
        opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        opts.AddFixedWindowLimiter("default", o =>
        {
            o.PermitLimit = rateLimitPermit;
            o.Window = TimeSpan.FromSeconds(rateLimitWindow);
            o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            o.QueueLimit = 0;
        });
        opts.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            RateLimitPartition.GetFixedWindowLimiter(
                ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitPermit,
                    Window = TimeSpan.FromSeconds(rateLimitWindow),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }));
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(opts =>
        {
            opts.Title = "QuickOrder API";
            opts.Theme = ScalarTheme.DeepSpace;
        });
    }

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<QuickOrder.Infrastructure.Persistence.AppDbContext>();
        await db.Database.MigrateAsync();
        await QuickOrder.Infrastructure.Persistence.AppDbContextSeed.SeedAsync(db);
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseCors();
    app.UseRateLimiter();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.MapControllers();
    app.MapRazorPages();
    app.MapHub<OrderHub>(OrderHub.Url);
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (ctx, report) =>
        {
            ctx.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
            });
            await ctx.Response.WriteAsync(result);
        }
    });

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "La aplicación terminó inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}

public class KebabCaseTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value) =>
        value is null ? null :
        Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
}
