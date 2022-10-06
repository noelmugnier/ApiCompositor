namespace ApiCompositor.Contracts;

public interface IRequestCompositor
{
    Task<CompositeResult> Compose<TU>(string requestId, ICompositeRequest<TU> compositeRequest, CancellationToken token);
}