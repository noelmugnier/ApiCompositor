namespace ApiCompositor.Contracts;

public interface ICompositorMapper<TApiRequest, TComposite, TCompositeResponse>
    where TApiRequest : IComposer
    where TComposite : IComposite<TCompositeResponse>
{
    public TComposite Map(TApiRequest request);
}