using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;

namespace ApiCompositor.Internal;

internal abstract class RequestComposerBase
{
    public abstract Task<ComposedResult<TU>> Compose<TU>(ICompositorProvider provider, object resource, CancellationToken token);
}
internal abstract class RequestComposerBaseWrapper<TResponse> : RequestComposerBase
{
    public abstract Task<ComposedResult<TU>> Compose<TU>(ICompositorProvider provider, IComposerRequest<TResponse> resource, CancellationToken token);
}

internal class RequestComposerBaseWrapperImpl<TRequest, TResponse>:RequestComposerBaseWrapper<TResponse>
    where TRequest : IComposerRequest<TResponse>
{

    public override async Task<ComposedResult<TU>> Compose<TU>(ICompositorProvider provider, object resource, CancellationToken token) =>
        await Compose<TU>(provider, (IComposerRequest<TResponse>) resource, token);
    
    public override async Task<ComposedResult<TU>> Compose<TU>(ICompositorProvider provider, IComposerRequest<TResponse> resource, CancellationToken token)
    {
        var requestDispatchers = provider.GetCompositeRequestDispatchers<TRequest, TResponse>();
        var tasks = new List<Task<ComposedResult>>();
        foreach (var requestDispatcher in requestDispatchers)
            tasks.Add(requestDispatcher.Dispatch(provider, resource, token));
        
        var results = await Task.WhenAll(tasks);
        var errors = new List<Error>();
        
        var composedResult = new ComposedResult<TU>();
        foreach (var result in results)
        {
            if(result.HasErrors)
                errors.AddRange(result.Errors);
            else
                foreach (var keyValue in result.AsDictionary())
                    composedResult[keyValue.Key] = keyValue.Value;
        }

        if (!errors.Any()) 
            return composedResult;
        
        foreach (var requestDispatcher in requestDispatchers)
            await requestDispatcher.OnError(provider, resource.RequestId, token);
            
        composedResult.SetErrors(errors);
        return composedResult;

    }
}