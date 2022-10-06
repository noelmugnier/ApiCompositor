using ApiCompositor;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Marketing;

public static class MarketingExtensions
{
    public static IServiceCollection AddMarketingHandlers(this IServiceCollection services)
    {
        services.RegisterAssemblyCompositeHandlers(typeof(MarketingProduct).Assembly);
        return services;
    }
}