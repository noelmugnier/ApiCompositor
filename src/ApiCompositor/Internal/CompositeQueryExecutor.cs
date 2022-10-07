using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composite;

namespace ApiCompositor.Internal;

internal class CompositeQueryExecutor<TCompositeQuery, TCompositeResponse> : ICompositeQueryExecutor<TCompositeQuery, TCompositeResponse> 
    where TCompositeQuery : ICompositeQuery<TCompositeResponse>
{
    private readonly ICompositorProvider _compositorProvider;

    public CompositeQueryExecutor(ICompositorProvider compositorProvider)
    {
        _compositorProvider = compositorProvider;
    }
    public async Task<ComposedResult> Execute(TCompositeQuery composite, CancellationToken token) 
    {
        var handler = _compositorProvider.GetCompositeQueryHandler<TCompositeQuery, TCompositeResponse>();
        var result = await handler.Handle(composite, token);
        return new ComposedResult(result);
    }

    public async Task<ComposedResult> ExecuteRequest<TCompositeRequest>(TCompositeRequest composite, CancellationToken token) 
        where TCompositeRequest : ICompositeRequest<TCompositeResponse>
    {
        var handler = _compositorProvider.GetCompositeRequestHandler<TCompositeRequest, TCompositeResponse>();
        var result = await handler.Handle(composite, token);
        return new ComposedResult(result);
    }
}