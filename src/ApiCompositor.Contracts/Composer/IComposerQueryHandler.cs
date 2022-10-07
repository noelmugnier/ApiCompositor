namespace ApiCompositor.Contracts.Composer;

public interface IComposerQueryHandler
{
    Task<ComposedResult<TU>> Compose<TU>(IComposerQuery<TU> query, CancellationToken token);
}