namespace ApiCompositor.Contracts.Composite;

public interface ICompositeRequestExecutor<in TCompositeRequest, TCompositeResponse>
    where TCompositeRequest : ICompositeRequest<TCompositeResponse>
{
    Task<ComposedResult> Execute(TCompositeRequest composite, CancellationToken token);
}