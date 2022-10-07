using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;

namespace Sample.Compositor.Contracts;

public record CreateComposerProduct(string RequestId, string Name, decimal Price, string? Description) : IComposerRequest<ProductViewModel>
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}