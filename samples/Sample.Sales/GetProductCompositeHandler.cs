using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composite;
using Sample.Compositor.Contracts;

namespace Sample.Sales;

public record GetSalesProduct(string RequestId, Guid Id) : ICompositeQuery<SalesProduct>
{
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}

public class GetProductCompositeHandler : ICompositeQueryHandler<GetSalesProduct, SalesProduct>
{
    public Task<SalesProduct> Handle(GetSalesProduct resource, CancellationToken token)
    {
        return Task.FromResult(new SalesProduct(resource.Id, 5));
    }
}

public class GetProductCompositorMapper : ICompositorMapper<GetComposerProduct, GetSalesProduct, SalesProduct>
{
    public GetSalesProduct Map(GetComposerProduct request)
    {
        return new GetSalesProduct(request.RequestId, request.Id);
    }
}