using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.Internal;

internal abstract class MainQueryHandlerBase
{
    public abstract Task<CompositeResult> Handle(IServiceProvider provider, string requestId, object resource, CancellationToken token);
}

internal abstract class MainQueryHandlerBaseWrapper<TResponse> : MainQueryHandlerBase
{
    public abstract Task<CompositeResult> Handle(IServiceProvider provider, string requestId, ICompositeQuery<TResponse> resource, CancellationToken token);
}

internal class MainQueryHandlerBaseWrapperImpl<TQuery, TResponse>:MainQueryHandlerBaseWrapper<TResponse>
    where TQuery : ICompositeQuery<TResponse>
{
    public override async Task<CompositeResult> Handle(IServiceProvider provider, string requestId, object resource, CancellationToken token) =>
        await Handle(provider, requestId, (ICompositeQuery<TResponse>) resource, token);
    
    public override async Task<CompositeResult> Handle(IServiceProvider provider, string requestId, ICompositeQuery<TResponse> resource, CancellationToken token)
    {
        var services = provider.GetServices(typeof(ICompositeQueryHandler)).ToList();
        var tasks = new List<Task<CompositeResult>>();
        foreach (var service in services)
        {
            var genericArguments = service
                .GetType()
                .GetInterfaces()
                .First(i => i.IsGenericType)
                .GetGenericArguments();

            var queryType = genericArguments.FirstOrDefault(ga => ga.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositeQuery<>)));
            if (queryType == null) continue;
            
            var responseType = genericArguments[1];
            
            var subQueryHandler = (SubQueryHandlerBase) Activator.CreateInstance(
                typeof(SubQueryHandlerBaseWrapperImpl<,>).MakeGenericType(queryType, responseType));
                
            tasks.Add(subQueryHandler.Handle(provider, requestId, resource, token));
        }
        
        var tasksResult = await Task.WhenAll(tasks);
        
        var dynamicResult = new CompositeResult();
        foreach (var taskResult in tasksResult)
        foreach (var keyValue in taskResult.AsDictionary())
            dynamicResult[keyValue.Key] = keyValue.Value;

        return dynamicResult;
    }
}