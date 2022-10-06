using ApiCompositor.Contracts;
using Microsoft.Extensions.Logging;
using Sample.Compositor.Contracts;

namespace Sample.Marketing;

public class CreateProductCompositeHandler : ICompositeRequestHandler<CreateProduct, MarketingProduct>
{
    private readonly ILogger<CreateProductCompositeHandler> _logger;

    public CreateProductCompositeHandler(ILogger<CreateProductCompositeHandler> logger)
    {
        _logger = logger;
    }

    public Task<MarketingProduct> Handle(string requestId, CreateProduct resource, CancellationToken token)
    {
        return Task.FromResult(new MarketingProduct(resource.Id, resource.Name));
    }

    public Task OnError(string requestId, CancellationToken token)
    {
        _logger.LogWarning("An error occured on request {RequestId}", requestId);
        return Task.CompletedTask;
    }
}