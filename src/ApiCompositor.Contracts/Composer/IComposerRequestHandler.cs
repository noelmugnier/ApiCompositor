namespace ApiCompositor.Contracts.Composer;

public interface IComposerRequestHandler
{
    Task<ComposedResult<TU>> Compose<TU>(IComposerRequest<TU> request, CancellationToken token);
}