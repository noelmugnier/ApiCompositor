using ApiCompositor.Contracts;
using ApiCompositor.Internal;

namespace ApiCompositor;

internal class QueryCompositor : IQueryCompositor
{
    private readonly IServiceProvider _provider;

    public QueryCompositor(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<CompositeResult> Compose<TU>(string requestId, ICompositeQuery<TU> request, CancellationToken token)
    {
        try
        {
            var handler = (MainQueryHandlerBase) Activator.CreateInstance(
                typeof(MainQueryHandlerBaseWrapperImpl<,>).MakeGenericType(request.GetType(), typeof(TU)));

            return await handler.Handle(_provider, requestId, request, token);
        }
        catch (Exception e)
        {
            return new CompositeResult();
        }
    }
}