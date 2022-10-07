using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;
using ApiCompositor.Contracts.Composite;
using ApiCompositor.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor;

internal class CompositorProvider : ICompositorProvider
{
    private readonly IServiceProvider _serviceProvider;

    public CompositorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IEnumerable<ICompositeQueryDispatcher> GetCompositeQueryDispatchers<TQuery, TResponse>() where TQuery : IComposerQuery<TResponse>
    {
        var services = _serviceProvider.GetServices(typeof(ICompositeQueryHandler)).ToList();
        var handlers = new List<CompositeQueryDispatcher>();
        foreach (var service in services)
        {
            var (compositeQueryType, compositeResponseType) = GetCompositeTypes(service.GetType(), typeof(IComposerQuery<>));

            handlers.Add((CompositeQueryDispatcher) Activator.CreateInstance(
                typeof(CompositeQueryDispatcherWrapperImpl<,,,>).MakeGenericType(typeof(TQuery), compositeQueryType, typeof(TResponse), compositeResponseType)));
        }

        return handlers;
    }

    public IEnumerable<ICompositeRequestDispatcher> GetCompositeRequestDispatchers<TRequest, TResponse>() where TRequest : IComposerRequest<TResponse>
    {
        var services = _serviceProvider.GetServices(typeof(ICompositeRequestHandler)).ToList();
        var handlers = new List<CompositeRequestDispatcher>();
        foreach (var service in services)
        {
            var (compositeType, compositeResponseType) = GetCompositeTypes(service.GetType(), typeof(IComposerRequest<>));
            
            handlers.Add((CompositeRequestDispatcher) Activator.CreateInstance(
                typeof(CompositeRequestDispatcherWrapperImpl<,,,>).MakeGenericType(typeof(TRequest), compositeType, typeof(TResponse), compositeResponseType)));
        }

        return handlers;
    }

    public ICompositeQueryHandler<TCompositeQuery, TCompositeResponse> GetCompositeQueryHandler<TCompositeQuery, TCompositeResponse>() where TCompositeQuery : ICompositeQuery<TCompositeResponse>
    {
        return _serviceProvider.GetService<ICompositeQueryHandler<TCompositeQuery, TCompositeResponse>>();
    }

    public ICompositorMapper<TComposerQuery, TCompositeQuery, TCompositeResponse> GetCompositorMapper<TComposerQuery, TCompositeQuery, TCompositeResponse>() where TComposerQuery : IComposer where TCompositeQuery : IComposite<TCompositeResponse>
    {
        return _serviceProvider.GetService<ICompositorMapper<TComposerQuery, TCompositeQuery, TCompositeResponse>>();
    }

    public ICompositeRequestHandler<TCompositeRequest, TCompositeResponse> GetCompositeRequestHandler<TCompositeRequest, TCompositeResponse>() where TCompositeRequest : ICompositeRequest<TCompositeResponse>
    {
        return _serviceProvider.GetService<ICompositeRequestHandler<TCompositeRequest, TCompositeResponse>>();
    }

    private (Type composite, Type response) GetCompositeTypes(Type serviceType, Type genericType) 
    {
        var genericArguments = serviceType
            .GetInterfaces()
            .First(i => i.IsGenericType)
            .GetGenericArguments();

        var compositeQueryType = genericArguments.FirstOrDefault(ga =>
            ga.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType));
        
        return compositeQueryType == null
            ? (null, null) 
            : (compositeQueryType, genericArguments[1]);
    }
}