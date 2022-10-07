namespace ApiCompositor.Contracts.Composer;

public interface IComposerRequestHandler
{
    Task<ComposedResult> Compose<TU>(IComposerRequest<TU> request, CancellationToken token);
}