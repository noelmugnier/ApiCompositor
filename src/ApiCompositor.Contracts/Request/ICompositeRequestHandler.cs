﻿namespace ApiCompositor.Contracts;

public interface ICompositeRequestHandler
{
}

public interface ICompositeRequestHandler<in TRequest, TResponse> : ICompositeRequestHandler
    where TRequest: ICompositeRequest<TResponse>
{
    Task<TResponse> Handle(TRequest resource, CancellationToken token);
    Task Revert(string requestId, CancellationToken token);
}