using System.Reflection;
using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor;

public static class DynamicMediatorExtensions
{
    public static IServiceCollection AddApiCompositor(this IServiceCollection services)
    {
        services.AddScoped<IQueryCompositor, QueryCompositor>();
        services.AddScoped<IRequestCompositor, RequestCompositor>();
        return services;
    }

    public static IServiceCollection RegisterAssemblyCompositeHandlers(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes();
        
        var requestHandlers = types.Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositeRequestHandler<,>)))
            .ToList();

        foreach (var requestHandler in requestHandlers)
        {
            services.AddScoped(typeof(ICompositeRequestHandler), requestHandler);
            services.AddScoped(requestHandler.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositeRequestHandler<,>)), requestHandler);
        }
        
        var queryHandlers = types.Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositeQueryHandler<,>)))
            .ToList();

        foreach (var queryHandler in queryHandlers)
        {
            services.AddScoped(typeof(ICompositeQueryHandler), queryHandler);
            services.AddScoped(queryHandler.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositeQueryHandler<,>)), queryHandler);
        }

        return services;
    }
}