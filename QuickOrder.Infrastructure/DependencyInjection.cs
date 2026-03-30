using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuickOrder.Application.Interfaces;
using QuickOrder.Infrastructure.Hubs;
using QuickOrder.Infrastructure.Messaging;
using QuickOrder.Infrastructure.Persistence;
using QuickOrder.Infrastructure.Repositories;
using QuickOrder.Infrastructure.Settings;

namespace QuickOrder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IMenuProductRepository, MenuProductRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IModifierGroupRepository, ModifierGroupRepository>();
        services.AddScoped<IModifierRepository, ModifierRepository>();

        services.AddScoped<IOrderHub, OrderHubService>();

        services.Configure<SqsOptions>(configuration.GetSection(SqsOptions.SectionName));
        services.AddSingleton<IAmazonSQS>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<SqsOptions>>().Value;
            var region = Amazon.RegionEndpoint.GetBySystemName(opts.Region);
            return new AmazonSQSClient(region);
        });
        services.AddScoped<IMessagePublisher, SqsMessagePublisher>();

        return services;
    }
}
