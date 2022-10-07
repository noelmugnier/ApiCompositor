using ApiCompositor.Contracts.Composer;

namespace ApiCompositor.Contracts.Composite;

public interface ICompositeQueryDispatcher
{
    Task<ComposedResult> Dispatch(ICompositorProvider provider, IComposer resource, CancellationToken token);
}