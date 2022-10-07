using ApiCompositor.Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Sample.Compositor.Contracts;

namespace Sample.Compositor.Api.Products;

[AllowAnonymous]
[HttpPost("/api/products")]
public class CreateProductEndpoint : Endpoint<CreateProductRequest>
{
    private readonly IComposerRequestHandler _composerRequestHandler;
    public CreateProductEndpoint(IComposerRequestHandler composerRequestHandler)
    {
        _composerRequestHandler = composerRequestHandler;
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var result = await _composerRequestHandler.Compose(new CreateComposerProduct(HttpContext.TraceIdentifier, req.Name, req.Price, req.Description), ct);
        await SendAsync(result.Fields, cancellation: ct);
    }
}

public record Test(Guid Id);
public record CreateProductRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}