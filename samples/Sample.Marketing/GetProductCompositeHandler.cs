using ApiCompositor.Contracts;
using Sample.Compositor.Contracts;

namespace Sample.Marketing;

public record GetMarketingProduct(string RequestId, Guid Id) : ICompositeQuery<MarketingProduct>
{
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}

public class GetProductCompositeHandler : ICompositeQueryHandler<GetMarketingProduct, MarketingProduct>
{
    public Task<MarketingProduct> Handle(GetMarketingProduct resource, CancellationToken token)
    {
        return Task.FromResult(new MarketingProduct(resource.Id, "Test"){Description = "Zeugma"});
    }
}

public class GetProductCompositorMapper : ICompositorMapper<GetComposerProduct, GetMarketingProduct, MarketingProduct>
{
    public GetMarketingProduct Map(GetComposerProduct request)
    {
        return new GetMarketingProduct(request.RequestId, request.Id);
    }
}