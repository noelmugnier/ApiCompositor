using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.Internal;

internal abstract class QueryComposerBase
{
    public abstract Task<ComposedResult> Compose(IServiceProvider provider, object resource, CancellationToken token);
}

internal abstract class QueryComposerBaseWrapper<TResponse> : QueryComposerBase
{
    public abstract Task<ComposedResult> Compose(IServiceProvider provider, IComposerQuery<TResponse> resource, CancellationToken token);
}

internal class QueryComposerBaseWrapperImpl<TQuery, TResponse>:QueryComposerBaseWrapper<TResponse>
    where TQuery : IComposerQuery<TResponse>
{
    public override async Task<ComposedResult> Compose(IServiceProvider provider, object resource, CancellationToken token) =>
        await Compose(provider, (IComposerQuery<TResponse>) resource, token);
    
    public override async Task<ComposedResult> Compose(IServiceProvider provider, IComposerQuery<TResponse> resource, CancellationToken token)
    {
        var services = provider.GetServices(typeof(ICompositeQueryHandler)).ToList();
        var tasks = new List<Task<ComposedResult>>();
        foreach (var service in services)
        {
            var genericArguments = service
                .GetType()
                .GetInterfaces()
                .First(i => i.IsGenericType)
                .GetGenericArguments();

            var compositeQueryType = genericArguments.FirstOrDefault(ga => ga.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICompositeQuery<>)));
            if (compositeQueryType == null) continue;
            
            var responseType = genericArguments[1];
            
            var queryCompositeBaseHandler = (CompositeQueryHandlerBase) Activator.CreateInstance(
                typeof(CompositeQueryHandlerBaseImpl<,,,>).MakeGenericType(typeof(TQuery), compositeQueryType, typeof(TResponse), responseType));
                
            tasks.Add(queryCompositeBaseHandler.Handle(provider, resource, token));
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
        
        result.SetErrors(errors);
        return result;
    }
}