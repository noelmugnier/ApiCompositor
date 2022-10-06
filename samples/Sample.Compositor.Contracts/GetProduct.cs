using ApiCompositor.Contracts;

namespace Sample.Compositor.Contracts;

public record GetProduct : ICompositeQuery<ProductViewModel>
{
    public Guid Id { get; init; }
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}