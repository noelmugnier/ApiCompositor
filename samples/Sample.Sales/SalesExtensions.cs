using ApiCompositor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Sales;

public static class SalesExtensions
{
    public static IServiceCollection AddSalesHandlers(this IServiceCollection services)
    {
        var assembly = typeof(SalesProduct).Assembly;
        
        services.RegisterAssemblyCompositeHandlers(assembly);
        services.RegisterAssemblyQueryExecutors(assembly);
        services.RegisterAssemblyRequestExecutors(assembly);
        
        return services;
    }
}