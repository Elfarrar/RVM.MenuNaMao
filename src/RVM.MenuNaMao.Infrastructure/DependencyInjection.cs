using RVM.MenuNaMao.Application.Services;
using RVM.MenuNaMao.Domain.Interfaces;
using RVM.MenuNaMao.Infrastructure.Data;
using RVM.MenuNaMao.Infrastructure.RabbitMQ;
using RVM.MenuNaMao.Infrastructure.RabbitMQ.Consumers;
using RVM.MenuNaMao.Infrastructure.Repositories;
using RVM.MenuNaMao.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RVM.MenuNaMao.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuNaMaoInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<MenuNaMaoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Repositories
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IStockItemRepository, StockItemRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();

        // RabbitMQ
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddHostedService<OrderPlacedConsumer>();
        services.AddHostedService<OrderStatusChangedConsumer>();
        services.AddHostedService<StockLowAlertConsumer>();

        // QR Code
        services.AddSingleton<IQrCodeService, QrCodeService>();

        return services;
    }
}
