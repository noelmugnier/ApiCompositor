namespace ApiCompositor.Contracts.Composite;

public interface ICompositeRequest<out TResponse> : IComposite<TResponse>
{
}