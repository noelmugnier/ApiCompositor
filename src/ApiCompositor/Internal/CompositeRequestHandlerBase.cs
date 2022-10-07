using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.Internal;

internal abstract class CompositeRequestHandlerBase
{
    public abstract Task<ComposedResult> Handle(IServiceProvider provider, object resource, CancellationToken token);
    public abstract Task<bool> Revert(IServiceProvider provider, string requestId, CancellationToken token);
}

internal class CompositeRequestHandlerBaseImpl<TComposerRequest, TCompositeRequest, TComposerResponse, TCompositeResponse> : CompositeRequestHandlerBase
    where TComposerRequest : IComposerRequest<TComposerResponse>
    where TCompositeRequest : ICompositeRequest<TCompositeResponse>
{
    public override async Task<ComposedResult> Handle(IServiceProvider provider, object resource,
        CancellationToken token)
    {
        try
        {
            var requestHandler = provider.GetService<ICompositeRequestHandler<TCompositeRequest, TCompositeResponse>>();
            var mapper = provider.GetService<ICompositorMapper<TComposerRequest,TCompositeRequest,TCompositeResponse>>();
            var result = await requestHandler.Handle(mapper.Map((TComposerRequest) resource), token);
            return new ComposedResult(result);
        }
        catch (Exception e)
        {
            var result = new ComposedResult();
            result.AddError(e.Source, e.Message);
            return result;
        }
    }
    
    public override async Task<bool> Revert(IServiceProvider provider, string requestId,
        CancellationToken token)
    {
        try
        {
            var requestHandler = provider.GetService<ICompositeRequestHandler<TCompositeRequest, TCompositeResponse>>();
            await requestHandler.Revert(requestId, token);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}