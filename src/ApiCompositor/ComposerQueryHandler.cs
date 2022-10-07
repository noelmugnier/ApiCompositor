using ApiCompositor.Contracts;
using ApiCompositor.Internal;

namespace ApiCompositor;

internal class ComposerQueryHandler : IComposerQueryHandler
{
    private readonly IServiceProvider _provider;

    public ComposerQueryHandler(IServiceProvider provider)
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