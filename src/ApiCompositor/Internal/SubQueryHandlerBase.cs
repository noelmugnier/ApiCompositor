using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.Internal;

internal abstract class SubQueryHandlerBase
{
    public abstract Task<CompositeResult> Handle(IServiceProvider provider, string requestId, object resource, CancellationToken token);
}

internal class SubQueryHandlerBaseWrapperImpl<TQuery, TResponse> : SubQueryHandlerBase
{
    public override async Task<CompositeResult> Handle(IServiceProvider provider, string requestId, object resource, CancellationToken token)
    {
        var queryHandler = provider.GetService<ICompositeQueryHandler<TQuery, TResponse>>();
        var result = await queryHandler.Handle(requestId, (TQuery)resource, token);
        return new CompositeResult(result);
    }
}