using ApiCompositor.Contracts;
using ApiCompositor.Internal;

namespace ApiCompositor;

internal class RequestCompositor : IRequestCompositor
{
    private readonly IServiceProvider _provider;

    public RequestCompositor(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<CompositeResult> Compose<TU>(string requestId, ICompositeRequest<TU> compositeRequest, CancellationToken token)
    {
        try
        {
            var handler = (MainRequestHandlerBase) Activator.CreateInstance(
                typeof(MainRequestHandlerBaseWrapperImpl<,>).MakeGenericType(compositeRequest.GetType(), typeof(TU)));

            return await handler.Handle(_provider, requestId, compositeRequest, token);
        }
        catch (Exception e)
        {
            return new CompositeResult();
        }
    }
}