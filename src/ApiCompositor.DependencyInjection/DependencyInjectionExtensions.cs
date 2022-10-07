using System.Reflection;
using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;
using ApiCompositor.Contracts.Composite;
using ApiCompositor.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApiCompositor(this IServiceCollection services)
    {
        services.AddScoped<ICompositorProvider, CompositorProvider>();
        services.AddScoped<IComposerQueryHandler, ComposerQueryHandler>();
        services.AddScoped<IComposerRequestHandler, ComposerRequestHandler>();
        return services;
    }

    public static IServiceCollection RegisterAssemblyRequestExecutors(this IServiceCollection services, Assembly assembly) 
        => RegisterAssemblyExecutors(services, assembly, typeof(ICompositeRequestExecutor<,>), typeof(CompositeRequestExecutor<,>), typeof(ICompositeRequest<>));
    
    public static IServiceCollection RegisterAssemblyQueryExecutors(this IServiceCollection services, Assembly assembly) 
        => RegisterAssemblyExecutors(services, assembly, typeof(ICompositeQueryExecutor<,>), typeof(CompositeQueryExecutor<,>), typeof(ICompositeQuery<>));
    
    public static IServiceCollection RegisterAssemblyRequestExecutors(this IServiceCollection services,
        Assembly assembly, Type executorImplementationType) 
        => RegisterAssemblyExecutors(services, assembly, typeof(ICompositeRequestExecutor<,>), executorImplementationType, typeof(ICompositeRequest<>));
    
    public static IServiceCollection RegisterAssemblyQueryExecutors(this IServiceCollection services,
        Assembly assembly, Type executorImplementationType) 
        => RegisterAssemblyExecutors(services, assembly, typeof(ICompositeQueryExecutor<,>), executorImplementationType, typeof(ICompositeQuery<>));
    
    private static IServiceCollection RegisterAssemblyExecutors(this IServiceCollection services, Assembly assembly, Type executorInterfaceType, Type executorImplementationType, Type compositeType)
    {
        var types = assembly.GetTypes();
        
        var compositeTypesToRegister = types.Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == compositeType))
            .ToList();

        foreach (var compositeTypeToRegister in compositeTypesToRegister)
        {
            var responseTypeArguments = compositeTypeToRegister
                .GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComposite<>))
                .GetGenericArguments();

            var responseType = responseTypeArguments[0];
            
            var executorInterface = executorInterfaceType.MakeGenericType(compositeTypeToRegister, responseType);
            var executor = executorImplementationType.MakeGenericType(compositeTypeToRegister, responseType);
            
            services.AddScoped(executorInterface, executor);
        }

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