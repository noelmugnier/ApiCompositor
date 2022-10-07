using ApiCompositor.Contracts;

namespace Sample.Compositor.Contracts;

public record GetComposerProduct(string RequestId, Guid Id) : IComposerQuery<ProductViewModel>
{
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}