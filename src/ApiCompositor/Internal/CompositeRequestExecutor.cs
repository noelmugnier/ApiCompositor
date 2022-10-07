using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composite;

namespace ApiCompositor.Internal;

internal class CompositeRequestExecutor<TCompositeRequest, TCompositeResponse> : ICompositeRequestExecutor<TCompositeRequest, TCompositeResponse> 
    where TCompositeRequest : ICompositeRequest<TCompositeResponse>
{
    private readonly ICompositorProvider _compositorProvider;

    public CompositeRequestExecutor(ICompositorProvider compositorProvider)
    {
        _compositorProvider = compositorProvider;
    }
    public async Task<ComposedResult> Execute(TCompositeRequest composite, CancellationToken token) 
    {
        var handler = _compositorProvider.GetCompositeRequestHandler<TCompositeRequest, TCompositeResponse>();
        var result = await handler.Handle(composite, token);
        return new ComposedResult(result);
    }
}