using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;
using ApiCompositor.Internal;

namespace ApiCompositor;

public class ComposerQueryHandler : IComposerQueryHandler
{
    private readonly ICompositorProvider _provider;

    public ComposerQueryHandler(ICompositorProvider provider)
    {
        _provider = provider;
    }

    public async Task<ComposedResult> Compose<TU>(IComposerQuery<TU> query, CancellationToken token)
    {
        try
        {
            var queryComposer = (QueryComposerBase) Activator.CreateInstance(
                typeof(QueryComposerBaseWrapperImpl<,>).MakeGenericType(query.GetType(), typeof(TU)));

            return await queryComposer.Compose(_provider, query, token);
        }
        catch (Exception e)
        {
            return new ComposedResult();
        }
    }
}