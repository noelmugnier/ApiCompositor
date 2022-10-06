using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.Internal;

internal abstract class SubRequestHandlerBase
{
    public abstract Task<CompositeResult> Handle(IServiceProvider provider, string requestId, object resource, CancellationToken token);
}

internal class SubRequestHandlerBaseWrapperImpl<TRequest, TResponse> : SubRequestHandlerBase
{
    public override async Task<CompositeResult> Handle(IServiceProvider provider, string requestId, object resource,
        CancellationToken token)
    {
        var requestHandler = provider.GetService<ICompositeRequestHandler<TRequest, TResponse>>();
        var result = await requestHandler.Handle(requestId, (TRequest)resource, token);
        return new CompositeResult(result);
    }
}