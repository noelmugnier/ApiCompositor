using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.Internal;

internal abstract class MainRequestHandlerBase
{
    public abstract Task<CompositeResult> Handle(IServiceProvider provider, string requestId, object resource, CancellationToken token);
}
internal abstract class MainRequestHandlerBaseWrapper<TResponse> : MainRequestHandlerBase
{
    public abstract Task<CompositeResult> Handle(IServiceProvider provider, string requestId, ICompositeRequest<TResponse> resource, CancellationToken token);
}

internal class MainRequestHandlerBaseWrapperImpl<TRequest, TResponse>:MainRequestHandlerBaseWrapper<TResponse>
    where TRequest : ICompositeRequest<TResponse>
{

    public override async Task<CompositeResult> Handle(IServiceProvider provider, string requestId, object resource, CancellationToken token) =>
        await Handle(provider, requestId, (ICompositeRequest<TResponse>) resource, token);
    
    public override async Task<CompositeResult> Handle(IServiceProvider provider, string requestId, ICompositeRequest<TResponse> resource, CancellationToken token)
    {
        var services = provider.GetServices(typeof(ICompositeRequestHandler)).ToList();
        var tasks = new List<Task<CompositeResult>>();
        foreach (var service in services)
        {
            var genericArguments = service
                .GetType()
                .GetInterfaces()
                .First(i => i.IsGenericType)
                .GetGenericArguments();

            var queryType = genericArguments.FirstOrDefault(ga => ga.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositeRequest<>)));
            if (queryType == null) continue;
            
            var responseType = genericArguments[1];
            
            var subRequesthandler = (SubRequestHandlerBase) Activator.CreateInstance(
                typeof(SubRequestHandlerBaseWrapperImpl<,>).MakeGenericType(queryType, responseType));
                
            tasks.Add(subRequesthandler.Handle(provider, requestId, resource, token));
        }
        
        var tasksResult = await Task.WhenAll(tasks);
        
        var dynamicResult = new CompositeResult();
        foreach (var taskResult in tasksResult)
        foreach (var keyValue in taskResult.AsDictionary())
            dynamicResult[keyValue.Key] = keyValue.Value;

        return dynamicResult;

    }
}