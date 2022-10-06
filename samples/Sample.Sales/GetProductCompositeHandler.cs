using ApiCompositor.Contracts;
using Sample.Compositor.Contracts;

namespace Sample.Sales;

public class GetProductCompositeHandler : ICompositeQueryHandler<GetProduct, SalesProduct>
{
    public Task<SalesProduct> Handle(string requestId, GetProduct resource, CancellationToken token)
    {
        return Task.FromResult(new SalesProduct(resource.Id, 5));
    }
}