namespace ApiCompositor.Contracts;

public interface IQueryCompositor
{
    Task<CompositeResult> Compose<TU>(string requestId, ICompositeQuery<TU> request, CancellationToken token);
}