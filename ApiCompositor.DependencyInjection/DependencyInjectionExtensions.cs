using System.Reflection;
using ApiCompositor;
using ApiCompositor.Contracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApiCompositor(this IServiceCollection services)
    {
        services.AddScoped<IComposerQueryHandler, ComposerQueryHandler>();
        services.AddScoped<IComposerRequestHandler, ComposerRequestHandler>();
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
        
        var mappers = types.Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositorMapper<,,>)))
            .ToList();

        foreach (var mapper in mappers)
        {
            services.AddScoped(mapper.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositorMapper<,,>)), mapper);
        }

        return services;
    }
}