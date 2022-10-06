namespace ApiCompositor.Contracts;

public interface ICompositeRequestHandler {}

public interface ICompositeRequestHandler<in TRequest, TResponse> :ICompositeRequestHandler
{
    Task<TResponse> Handle(string requestId, TRequest resource, CancellationToken token);
    Task OnError(string requestId, CancellationToken token);
}