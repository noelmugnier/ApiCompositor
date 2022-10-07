using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composite;
using MediatR;
using Microsoft.Extensions.Logging;
using Sample.Compositor.Contracts;

namespace Sample.Marketing;

public record CreateMarketingProduct(string RequestId, Guid Id, string Name, string? Description = null) 
    : ICompositeRequest<MarketingProduct>, IRequest<MarketingProduct>
{
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}

public class CreateProductCompositeHandler : 
    ICompositeRequestHandler<CreateMarketingProduct, MarketingProduct>,
    IRequestHandler<CreateMarketingProduct, MarketingProduct>
{
    private readonly ILogger<CreateProductCompositeHandler> _logger;

    public CreateProductCompositeHandler(ILogger<CreateProductCompositeHandler> logger)
    {
        _logger = logger;
    }

    public Task<MarketingProduct> Handle(CreateMarketingProduct resource, CancellationToken token)
    {
        return Task.FromResult(new MarketingProduct(resource.Id, resource.Name, "Bags"));
    }

    public Task<bool> Revert(string requestId, CancellationToken token)
    {
        _logger.LogWarning("An error occured on request {RequestId}", requestId);
        return Task.FromResult(true);
    }
}

public class CreateProductCompositorMapper : ICompositorMapper<CreateComposerProduct, CreateMarketingProduct, MarketingProduct>
{
    public CreateMarketingProduct Map(CreateComposerProduct request)
    {
        return new CreateMarketingProduct(request.RequestId, request.Id, request.Name, request.Description);
    }
}