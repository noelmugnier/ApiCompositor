using ApiCompositor.Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Sample.Compositor.Contracts;

namespace Sample.Compositor.Api.Products;

[AllowAnonymous]
[HttpGet("/api/products/{Id}")]
public class GetProductEndpoint : Endpoint<GetProductQuery>
{
    private readonly IComposerQueryHandler _requestHandler;
    public GetProductEndpoint(IComposerQueryHandler requestHandler)
    {
        _requestHandler = requestHandler;
    }

    public override async Task HandleAsync(GetProductQuery req, CancellationToken ct)
    {
        var result = await _requestHandler.Compose(new GetComposerProduct(HttpContext.TraceIdentifier, req.Id), ct);
        await SendAsync(result.Fields, cancellation: ct);
    }
}

public record GetProductQuery
{
    public Guid Id { get; set; }
}