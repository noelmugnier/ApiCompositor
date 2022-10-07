namespace ApiCompositor.Contracts;

public interface ICompositeQueryHandler
{
}

public interface ICompositeQueryHandler<in TQuery, TResponse> : ICompositeQueryHandler
    where TQuery: ICompositeQuery<TResponse>
{
    Task<TResponse> Handle(TQuery resource, CancellationToken token);
}