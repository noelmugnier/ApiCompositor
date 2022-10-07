using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composite;

namespace Sample.Marketing;

public class MediatorQueryExecutor<TQuery, TResponse> : ICompositeQueryExecutor<TQuery, TResponse> 
    where TQuery : ICompositeQuery<TResponse>
{
    public Task<ComposedResult> Execute(TQuery composite, CancellationToken token)
    {
        return Task.FromResult(new ComposedResult());
    }
}

public class MediatorRequestExecutor<TRequest, TResponse> : ICompositeRequestExecutor<TRequest, TResponse> 
    where TRequest : ICompositeRequest<TResponse>
{
    public Task<ComposedResult> Execute(TRequest composite, CancellationToken token)
    {
        return Task.FromResult(new ComposedResult());
    }
}