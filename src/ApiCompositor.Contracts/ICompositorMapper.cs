using ApiCompositor.Contracts.Composer;
using ApiCompositor.Contracts.Composite;

namespace ApiCompositor.Contracts;

public interface ICompositorMapper<in TApiRequest, out TComposite, TCompositeResponse>
    where TApiRequest : IComposer
    where TComposite : IComposite<TCompositeResponse>
{
    public TComposite Map(TApiRequest request);
}