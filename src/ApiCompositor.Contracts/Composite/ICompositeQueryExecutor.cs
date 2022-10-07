namespace ApiCompositor.Contracts.Composite;

public interface ICompositeQueryExecutor<in TCompositeQuery, TCompositeResponse>
    where TCompositeQuery : ICompositeQuery<TCompositeResponse>
{
    Task<ComposedResult> Execute(TCompositeQuery composite, CancellationToken token);
}