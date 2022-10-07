namespace ApiCompositor.Contracts.Composer;

public interface IComposerQueryHandler
{
    Task<ComposedResult> Compose<TU>(IComposerQuery<TU> query, CancellationToken token);
}