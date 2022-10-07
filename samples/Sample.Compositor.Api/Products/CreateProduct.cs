using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using Sample.Compositor.Contracts;

namespace Sample.Compositor.Api.Products;

public class CreateProductEndpoint : Endpoint<CreateProductRequest>
{
    private readonly IComposerRequestHandler _composerRequestHandler;
    public CreateProductEndpoint(IComposerRequestHandler composerRequestHandler)
    {
        _composerRequestHandler = composerRequestHandler;
    }
    
    public override void Configure()
    {
        Post("/products");
        AllowAnonymous();
        Description(b => b
                .WithGroupName("Products")
                .WithDisplayName("CreateProduct")
                .Accepts<CreateProductRequest>("application/json")
                .Produces<ProductViewModel>(201, "application/json")
                .ProducesProblem(400)
                .ProducesProblemFE<InternalErrorResponse>(500),
            clearDefaults: true);
        Summary(s => {
            s.Summary = "Create a new product";
            s.Description = "Call multiple services (sales/marketing) to create product";
            s.ExampleRequest = new CreateProductRequest { Name = "test", Price = 5};
        });
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var result = await _composerRequestHandler.Compose(new CreateComposerProduct(HttpContext.TraceIdentifier, req.Name, req.Price, req.Description), ct);
        if (result.HasErrors)
        {
            var problem = new ProblemDetails() {Status = 400, Title = "An error occured while creating product"};
            problem.Extensions.Add("Errors", result.Errors);
            
            await SendAsync(problem, problem.Status.Value, ct);
            return;
        }

        await SendCreatedAtAsync<GetProductEndpoint>(new { result.Instance.Id }, result, cancellation: ct);
    }
}

public record CreateProductRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}