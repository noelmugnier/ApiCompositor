namespace ApiCompositor.Contracts;

public interface IComposerQueryHandler
{
    Task<ComposedResult> Compose<TU>(IComposerQuery<TU> query, CancellationToken token);
}