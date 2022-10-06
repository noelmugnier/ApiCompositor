using ApiCompositor.Contracts;

namespace Sample.Compositor.Contracts;

public record CreateProduct : ICompositeRequest<ProductViewModel>
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}