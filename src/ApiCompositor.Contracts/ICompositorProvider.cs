using ApiCompositor.Contracts.Composer;
using ApiCompositor.Contracts.Composite;

namespace ApiCompositor.Contracts;

public interface ICompositorProvider
{
    IEnumerable<ICompositeQueryDispatcher> GetCompositeQueryDispatchers<TQuery, TResponse>() 
        where TQuery : IComposerQuery<TResponse>;
    IEnumerable<ICompositeRequestDispatcher> GetCompositeRequestDispatchers<TRequest, TResponse>() 
        where TRequest : IComposerRequest<TResponse>;
    ICompositeQueryHandler<TCompositeQuery, TCompositeResponse> GetCompositeQueryHandler<TCompositeQuery, TCompositeResponse>() 
        where TCompositeQuery : ICompositeQuery<TCompositeResponse>;
    ICompositeRequestHandler<TCompositeRequest, TCompositeResponse>  GetCompositeRequestHandler<TCompositeRequest, TCompositeResponse>() 
        where TCompositeRequest : ICompositeRequest<TCompositeResponse>;
    ICompositorMapper<TComposer, TComposite, TCompositeResponse> GetCompositorMapper<TComposer, TComposite, TCompositeResponse>() 
        where TComposer : IComposer
        where TComposite : IComposite<TCompositeResponse>;
    ICompositeQueryExecutor<TComposite, TCompositeResponse> GetCompositeQueryExecutor<TComposite, TCompositeResponse>() where TComposite : ICompositeQuery<TCompositeResponse>;
    ICompositeRequestExecutor<TComposite, TCompositeResponse> GetCompositeRequestExecutor<TComposite, TCompositeResponse>() where TComposite : ICompositeRequest<TCompositeResponse>;
}