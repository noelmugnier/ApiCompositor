using ApiCompositor.Contracts.Composite;
using ApiCompositor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Marketing;

public static class MarketingExtensions
{
    public static IServiceCollection AddMarketingHandlers(this IServiceCollection services)
    {
        services.RegisterAssemblyCompositeHandlers(typeof(MarketingProduct).Assembly);
        services.RegisterAssemblyQueryExecutors(typeof(MarketingProduct).Assembly, typeof(MediatorQueryExecutor<,>));
        services.RegisterAssemblyRequestExecutors(typeof(MarketingProduct).Assembly, typeof(MediatorRequestExecutor<,>));
        return services;
    }
}