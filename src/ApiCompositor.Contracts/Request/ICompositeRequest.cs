﻿namespace ApiCompositor.Contracts;

public interface ICompositeRequest<out TResponse> : IComposite<TResponse>
{
}