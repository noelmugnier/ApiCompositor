using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composite;
using MediatR;

namespace Sample.Marketing;

public class MediatorQueryExecutor<TQuery, TResponse> : ICompositeQueryExecutor<TQuery, TResponse> 
    where TQuery : ICompositeQuery<TResponse>, IRequest<TResponse>
{
    private readonly IMediator _mediator;

    public MediatorQueryExecutor(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task<ComposedResult> Execute(TQuery composite, CancellationToken token)
    {
        var result = await _mediator.Send(composite, token);
        return new ComposedResult(result);
    }
}

public class MediatorRequestExecutor<TRequest, TResponse> : ICompositeRequestExecutor<TRequest, TResponse> 
    where TRequest : ICompositeRequest<TResponse>, IRequest<TResponse>
{
    private readonly IMediator _mediator;

    public MediatorRequestExecutor(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task<ComposedResult> Execute(TRequest composite, CancellationToken token)
    {
        var result = await _mediator.Send(composite, token);
        return new ComposedResult(result);
    }
}