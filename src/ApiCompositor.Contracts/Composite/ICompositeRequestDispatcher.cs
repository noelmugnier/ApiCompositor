using ApiCompositor.Contracts.Composer;

namespace ApiCompositor.Contracts.Composite;

public interface ICompositeRequestDispatcher
{
    Task<ComposedResult> Dispatch(ICompositorProvider provider, IComposer resource, CancellationToken token);
    Task<bool> OnError(ICompositorProvider provider, string requestId, CancellationToken token);
}