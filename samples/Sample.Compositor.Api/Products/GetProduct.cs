using ApiCompositor.Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Sample.Compositor.Contracts;

namespace Sample.Compositor.Api.Products;

[AllowAnonymous]
[HttpGet("/api/products/{Id}")]
public class GetProductEndpoint : Endpoint<GetProduct>
{
    private readonly IQueryCompositor _requestCompositor;
    public GetProductEndpoint(IQueryCompositor requestCompositor)
    {
        _requestCompositor = requestCompositor;
    }

    public override async Task HandleAsync(GetProduct req, CancellationToken ct)
    {
        var result = await _requestCompositor.Compose(HttpContext.TraceIdentifier, req, ct);
        await SendAsync(result.Fields, cancellation: ct);
    }
}