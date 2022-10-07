using ApiCompositor.Contracts.Composer;
using FastEndpoints;
using Sample.Compositor.Contracts;

namespace Sample.Compositor.Api.Products;

public class GetProductEndpoint : Endpoint<GetProductQuery>
{
    private readonly IComposerQueryHandler _requestHandler;
    public GetProductEndpoint(IComposerQueryHandler requestHandler)
    {
        _requestHandler = requestHandler;
    }
    
    public override void Configure()
    {
        Get("/products/{Id}");
        AllowAnonymous();
        Description(b => b
                .WithGroupName("Products")
                .WithDisplayName("GetProduct")
                .Accepts<GetProductQuery>("application/json")
                .Produces<ProductViewModel>(200, "application/json")
                .ProducesProblem(400)
                .ProducesProblem(404)
                .ProducesProblemFE<InternalErrorResponse>(500),
        clearDefaults: true);
        Summary(s => {
            s.Summary = "Retrieve a product with id";
            s.Description = "Call multiple services (sales/marketing) to build ProductViewModel";
            s.ExampleRequest = new GetProductQuery { Id = Guid.NewGuid()};
        });
    }

    public override async Task HandleAsync(GetProductQuery req, CancellationToken ct)
    {
        var result = await _requestHandler.Compose(new GetComposerProduct(HttpContext.TraceIdentifier, req.Id), ct);
        await SendAsync(result, cancellation: ct);
    }
}

public record GetProductQuery
{
    public Guid Id { get; set; }
}