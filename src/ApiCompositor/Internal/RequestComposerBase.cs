using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.Internal;

internal abstract class RequestComposerBase
{
    public abstract Task<ComposedResult> Compose(IServiceProvider provider, object resource, CancellationToken token);
}
internal abstract class RequestComposerBaseWrapper<TResponse> : RequestComposerBase
{
    public abstract Task<ComposedResult> Compose(IServiceProvider provider, IComposerRequest<TResponse> resource, CancellationToken token);
}

internal class RequestComposerBaseWrapperImpl<TRequest, TResponse>:RequestComposerBaseWrapper<TResponse>
    where TRequest : IComposerRequest<TResponse>
{

    public override async Task<ComposedResult> Compose(IServiceProvider provider, object resource, CancellationToken token) =>
        await Compose(provider, (IComposerRequest<TResponse>) resource, token);
    
    public override async Task<ComposedResult> Compose(IServiceProvider provider, IComposerRequest<TResponse> resource, CancellationToken token)
    {
        var services = provider.GetServices(typeof(ICompositeRequestHandler)).ToList();
        var tasks = new List<Task<ComposedResult>>();
        var handlers = new List<CompositeRequestHandlerBase>();
        foreach (var service in services)
        {
            var genericArguments = service
                .GetType()
                .GetInterfaces()
                .First(i => i.IsGenericType)
                .GetGenericArguments();

            var compositeRequestType = genericArguments.FirstOrDefault(ga => ga.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositeRequest<>)));
            if (compositeRequestType == null) continue;
            
            var responseType = genericArguments[1];
            
            var requestCompositeBaseHandler = (CompositeRequestHandlerBase) Activator.CreateInstance(
                typeof(CompositeRequestHandlerBaseImpl<,,,>).MakeGenericType(typeof(TRequest), compositeRequestType, typeof(TResponse), responseType));
            handlers.Add(requestCompositeBaseHandler);
                
            tasks.Add(requestCompositeBaseHandler.Handle(provider, resource, token));
        }
        
        var tasksResult = await Task.WhenAll(tasks);
        var errors = new List<KeyValuePair<string, object>>();
        
        var result = new ComposedResult();
        foreach (var taskResult in tasksResult)
        {
            if(taskResult.HasErrors)
                errors.AddRange(taskResult.Errors);
            
            foreach (var keyValue in taskResult.AsDictionary())
                result[keyValue.Key] = keyValue.Value;
        }

        if (!errors.Any()) 
            return result;
        
        foreach (var handler in handlers)
            await handler.Revert(provider, resource.RequestId, token);
            
        result.SetErrors(errors);
        return result;

    }
}