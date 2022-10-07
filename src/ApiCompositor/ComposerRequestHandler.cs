using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;
using ApiCompositor.Internal;

namespace ApiCompositor;

public class ComposerRequestHandler : IComposerRequestHandler
{
    private readonly ICompositorProvider _provider;

    public ComposerRequestHandler(ICompositorProvider provider)
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