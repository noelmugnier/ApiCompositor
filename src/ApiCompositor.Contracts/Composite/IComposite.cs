namespace ApiCompositor.Contracts.Composite;

public interface IComposite<out TResponse>{
    string RequestId { get; }
    DateTimeOffset RequestedOn { get; }
}