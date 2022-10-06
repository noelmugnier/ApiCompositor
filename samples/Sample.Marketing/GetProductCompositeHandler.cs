using ApiCompositor.Contracts;
using Sample.Compositor.Contracts;

namespace Sample.Marketing;

public class GetProductCompositeHandler : ICompositeQueryHandler<GetProduct, MarketingProduct>
{
    public Task<MarketingProduct> Handle(string requestId, GetProduct resource, CancellationToken token)
    {
        return Task.FromResult(new MarketingProduct(resource.Id, "Test"){Description = "Zeugma"});
    }
}