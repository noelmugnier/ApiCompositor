using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;

namespace ApiCompositor.Internal;

internal abstract class QueryComposerBase
{
    public abstract Task<ComposedResult<TU>> Compose<TU>(ICompositorProvider provider, object resource, CancellationToken token);
}

internal abstract class QueryComposerBaseWrapper<TResponse> : QueryComposerBase
{
    public abstract Task<ComposedResult<TU>> Compose<TU>(ICompositorProvider provider, IComposerQuery<TResponse> resource, CancellationToken token);
}

internal class QueryComposerBaseWrapperImpl<TQuery, TResponse>:QueryComposerBaseWrapper<TResponse>
    where TQuery : IComposerQuery<TResponse>
{
    public override async Task<ComposedResult<TU>> Compose<TU>(ICompositorProvider provider, object resource, CancellationToken token) =>
        await Compose<TU>(provider, (IComposerQuery<TResponse>) resource, token);
    
    public override async Task<ComposedResult<TU>> Compose<TU>(ICompositorProvider provider, IComposerQuery<TResponse> resource, CancellationToken token)
    {
        var queryDispatchers = provider.GetCompositeQueryDispatchers<TQuery, TResponse>();
        var tasks = new List<Task<ComposedResult>>();
        foreach (var queryDispatcher in queryDispatchers)
            tasks.Add(queryDispatcher.Dispatch(provider, resource, token));
        
        var results = await Task.WhenAll(tasks);
        var errors = new List<Error>();
        
        var composedResult = new ComposedResult<TU>();
        foreach (var result in results)
        {
            if(result.HasErrors)
                errors.AddRange(result.Errors);
            
            foreach (var keyValue in result.AsDictionary())
                composedResult[keyValue.Key] = keyValue.Value;
        }

        if (!errors.Any()) 
            return composedResult;
        
        composedResult.SetErrors(errors);
        return composedResult;
    }
}