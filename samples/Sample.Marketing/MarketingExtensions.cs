using ApiCompositor.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Marketing;

public static class MarketingExtensions
{
    public static IServiceCollection AddMarketingHandlers(this IServiceCollection services)
    {
        var assembly = typeof(MarketingProduct).Assembly;
        
        services.RegisterAssemblyCompositeHandlers(assembly);
        services.RegisterAssemblyQueryExecutors(assembly, typeof(MediatorQueryExecutor<,>));
        services.RegisterAssemblyRequestExecutors(assembly, typeof(MediatorRequestExecutor<,>));
        services.AddMediatR(assembly);
        
        return services;
    }
}