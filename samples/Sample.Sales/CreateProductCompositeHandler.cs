using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composite;
using Microsoft.Extensions.Logging;
using Sample.Compositor.Contracts;

namespace Sample.Sales;

public record CreateSalesProduct(string RequestId, Guid Id, decimal Price) : ICompositeRequest<SalesProduct>
{
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}

public class CreateProductCompositeHandler : ICompositeRequestHandler<CreateSalesProduct, SalesProduct>
{
    private readonly ILogger<CreateProductCompositeHandler> _logger;

    public CreateProductCompositeHandler(ILogger<CreateProductCompositeHandler> logger)
    {
        _logger = logger;
    }
        
    public Task<SalesProduct> Handle(CreateSalesProduct resource, CancellationToken token)
    {
        return Task.FromResult(new SalesProduct(resource.Id, resource.Price));
    }

    public Task Revert(string requestId, CancellationToken token)
    {
        _logger.LogWarning($"An error occured on request {requestId}");
        return Task.CompletedTask;
    }
}

public class CreateProductCompositorMapper : ICompositorMapper<CreateComposerProduct, CreateSalesProduct, SalesProduct>
{
    public CreateSalesProduct Map(CreateComposerProduct request)
    {
        return new CreateSalesProduct(request.RequestId, request.Id, request.Price);
    }
}