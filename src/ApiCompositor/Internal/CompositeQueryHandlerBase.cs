using ApiCompositor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCompositor.Internal;

internal abstract class CompositeQueryHandlerBase
{
    public abstract Task<ComposedResult> Handle(IServiceProvider provider, object resource, CancellationToken token);
}

internal class CompositeQueryHandlerBaseImpl<TComposerQuery, TCompositeQuery, TComposerResponse, TCompositeResponse> : CompositeQueryHandlerBase
    where TComposerQuery : IComposerQuery<TComposerResponse>
    where TCompositeQuery : ICompositeQuery<TCompositeResponse>
{
    public override async Task<ComposedResult> Handle(IServiceProvider provider, object resource, CancellationToken token)
    {
        try
        {
            var queryHandler = provider.GetService<ICompositeQueryHandler<TCompositeQuery, TCompositeResponse>>();
            var mapper = provider.GetService<ICompositorMapper<TComposerQuery,TCompositeQuery,TCompositeResponse>>();
            var result = await queryHandler.Handle(mapper.Map((TComposerQuery)resource), token);
            return new ComposedResult(result);
        }
        catch (Exception e)
        {
            var result = new ComposedResult();
            result.AddError(e.Source, e.Message);
            return result;
        }
    }
}