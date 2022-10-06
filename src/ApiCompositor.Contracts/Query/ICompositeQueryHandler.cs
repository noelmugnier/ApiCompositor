namespace ApiCompositor.Contracts;

public interface ICompositeQueryHandler
{
}

public interface ICompositeQueryHandler<in TQuery, TResponse> : ICompositeQueryHandler
{
    Task<TResponse> Handle(string requestId, TQuery resource, CancellationToken token);
}