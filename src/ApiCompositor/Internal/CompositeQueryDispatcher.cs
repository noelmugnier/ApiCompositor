﻿using ApiCompositor.Contracts;
using ApiCompositor.Contracts.Composer;
using ApiCompositor.Contracts.Composite;

namespace ApiCompositor.Internal;


internal abstract class CompositeQueryDispatcher : ICompositeQueryDispatcher
{
    public abstract Task<ComposedResult> Dispatch(ICompositorProvider provider, IComposer resource, CancellationToken token);
}

internal abstract class CompositeQueryDispatcherWrapper<TComposerQuery, TComposerResponse> : CompositeQueryDispatcher
    where TComposerQuery : IComposerQuery<TComposerResponse>
{
    public abstract Task<ComposedResult> Handle(ICompositorProvider provider, TComposerQuery resource, CancellationToken token);
}

internal class CompositeQueryDispatcherWrapperImpl<TComposerQuery, TCompositeQuery, TComposerResponse, TCompositeResponse> : CompositeQueryDispatcherWrapper<TComposerQuery, TComposerResponse>
    where TComposerQuery : IComposerQuery<TComposerResponse>
    where TCompositeQuery : ICompositeQuery<TCompositeResponse>
{
    public override async Task<ComposedResult> Dispatch(ICompositorProvider provider, IComposer resource, CancellationToken token)
        => await Handle(provider, (TComposerQuery) resource, token);

    public override async Task<ComposedResult> Handle(ICompositorProvider provider, TComposerQuery resource, CancellationToken token)
    {
        try
        {
            var queryHandler = provider.GetCompositeQueryHandler<TCompositeQuery, TCompositeResponse>();
            var mapper = provider.GetCompositorMapper<TComposerQuery, TCompositeQuery, TCompositeResponse>();
            var result = await queryHandler.Handle(mapper.Map(resource), token);
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