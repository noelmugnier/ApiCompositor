using ApiCompositor.Contracts;
using ApiCompositor.Internal;

namespace ApiCompositor;

internal class ComposerRequestHandler : IComposerRequestHandler
{
    private readonly IServiceProvider _provider;

    public ComposerRequestHandler(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<ComposedResult> Compose<TU>(IComposerRequest<TU> request, CancellationToken token)
    {
        try
        {
            var requestComposer = (RequestComposerBase) Activator.CreateInstance(
                typeof(RequestComposerBaseWrapperImpl<,>).MakeGenericType(request.GetType(), typeof(TU)));

            return await requestComposer.Compose(_provider, request, token);
        }
        catch (Exception e)
        {
            return new ComposedResult();
        }
    }
}