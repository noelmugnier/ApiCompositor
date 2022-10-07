using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;

namespace Sample.Compositor.Contracts;

public record GetComposerProduct(string RequestId, Guid Id) : IComposerQuery<ProductViewModel>
{
    public DateTimeOffset RequestedOn { get; } = DateTimeOffset.UtcNow;
}