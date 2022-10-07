using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;
using ApiCompositor.Contracts.Composite;

namespace ApiCompositor.Internal;

internal abstract class CompositeRequestDispatcher : ICompositeRequestDispatcher
{
    public abstract Task<ComposedResult> Dispatch(ICompositorProvider provider, IComposer resource, CancellationToken token);
    public abstract Task<bool> OnError(ICompositorProvider provider, string requestId, CancellationToken token);
}

internal abstract class CompositeRequestDispatcherWrapper<TComposerRequest, TComposerResponse> : CompositeRequestDispatcher
    where TComposerRequest : IComposerRequest<TComposerResponse>
{
    public abstract Task<ComposedResult> Handle(ICompositorProvider provider, TComposerRequest resource, CancellationToken token);
}

internal class CompositeRequestDispatcherWrapperImpl<TComposerRequest, TCompositeRequest, TComposerResponse, TCompositeResponse> : CompositeRequestDispatcherWrapper<TComposerRequest, TComposerResponse>
    where TComposerRequest : IComposerRequest<TComposerResponse>
    where TCompositeRequest : ICompositeRequest<TCompositeResponse>
{
    public override async Task<ComposedResult> Dispatch(ICompositorProvider provider, IComposer resource, CancellationToken token)
        => await Handle(provider, (TComposerRequest)resource, token);
    
    public override async Task<ComposedResult> Handle(ICompositorProvider provider, TComposerRequest resource, CancellationToken token)
    {
        try
        {
            var mapper = provider.GetCompositorMapper<TComposerRequest, TCompositeRequest, TCompositeResponse>();
            var composite = mapper.Map(resource);
            
            var executor = provider.GetCompositeRequestExecutor<TCompositeRequest, TCompositeResponse>();
            return await executor.Execute(composite, token);
        }
        catch (Exception e)
        {
            var result = new ComposedResult();
            result.AddError(e.Source, e.Message);
            return result;
        }
    }
    
    public override async Task<bool> OnError(ICompositorProvider provider, string requestId,
        CancellationToken token)
    {
        try
        {
            var requestHandler = provider.GetCompositeRequestHandler<TCompositeRequest, TCompositeResponse>();
            await requestHandler.Revert(requestId, token);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}