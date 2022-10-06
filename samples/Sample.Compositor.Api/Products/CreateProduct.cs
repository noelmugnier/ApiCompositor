using ApiCompositor.Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Sample.Compositor.Contracts;

namespace Sample.Compositor.Api.Products;

[AllowAnonymous]
[HttpPost("/api/products")]
public class CreateProductEndpoint : Endpoint<CreateProduct>
{
    private readonly IRequestCompositor _requestCompositor;
    public CreateProductEndpoint(IRequestCompositor requestCompositor)
    {
        _requestCompositor = requestCompositor;
    }

    public override async Task HandleAsync(CreateProduct req, CancellationToken ct)
    {
        var result = await _requestCompositor.Compose(HttpContext.TraceIdentifier, req, ct);
        await SendAsync(result.Fields, cancellation: ct);
    }
}