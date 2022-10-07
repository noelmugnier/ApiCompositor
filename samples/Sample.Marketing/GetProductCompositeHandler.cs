using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composite;
using MediatR;
using Sample.Compositor.Contracts;

namespace Sample.Marketing;

public record GetMarketingProduct(string RequestId, Guid Id)
    : ICompositeQuery<MarketingProduct>, IRequest<MarketingProduct>
{
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}

public class GetProductCompositeHandler : 
    ICompositeQueryHandler<GetMarketingProduct, MarketingProduct>,
    IRequestHandler<GetMarketingProduct, MarketingProduct>
{
    public Task<MarketingProduct> Handle(GetMarketingProduct resource, CancellationToken token)
    {
        return Task.FromResult(new MarketingProduct(resource.Id, "Test", "Bags"){Description = "Zeugma"});
    }
}

public class GetProductCompositorMapper : ICompositorMapper<GetComposerProduct, GetMarketingProduct, MarketingProduct>
{
    public GetMarketingProduct Map(GetComposerProduct request)
    {
        return new GetMarketingProduct(request.RequestId, request.Id);
    }
}