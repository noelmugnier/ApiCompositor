using ApiCompositor.Contracts;
using Microsoft.Extensions.Logging;
using Sample.Compositor.Contracts;

namespace Sample.Sales;

public class CreateProductCompositeHandler : ICompositeRequestHandler<CreateProduct, SalesProduct>
{
    private readonly ILogger<CreateProductCompositeHandler> _logger;

    public CreateProductCompositeHandler(ILogger<CreateProductCompositeHandler> logger)
    {
        _logger = logger;
    }
        
    public Task<SalesProduct> Handle(string requestId, CreateProduct resource, CancellationToken token)
    {
        return Task.FromResult(new SalesProduct(resource.Id, resource.Price));
    }

    public Task OnError(string requestId, CancellationToken token)
    {
        _logger.LogWarning($"An error occured on request {requestId}");
        return Task.CompletedTask;
    }
}