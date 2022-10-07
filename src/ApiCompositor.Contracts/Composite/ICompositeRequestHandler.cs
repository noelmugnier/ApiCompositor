namespace ApiCompositor.Contracts.Composite;

public interface ICompositeRequestHandler
{
}

public interface ICompositeRequestHandler<in TRequest, TResponse> : ICompositeRequestHandler
    where TRequest: ICompositeRequest<TResponse>
{
    Task<TResponse> Handle(TRequest resource, CancellationToken token);
    Task<bool> Revert(string requestId, CancellationToken token);
}