using ApiCompositor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Sales;

public static class SalesExtensions
{
    public static IServiceCollection AddSalesHandlers(this IServiceCollection services)
    {
        services.RegisterAssemblyCompositeHandlers(typeof(SalesProduct).Assembly);
        services.RegisterAssemblyQueryExecutors(typeof(SalesProduct).Assembly);
        services.RegisterAssemblyRequestExecutors(typeof(SalesProduct).Assembly);
        return services;
    }
}