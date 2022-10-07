namespace ApiCompositor.Contracts;

public interface IComposite<out TResponse>{
    string RequestId { get; }
    DateTimeOffset RequestedOn { get; }
}
public interface IComposer{
    string RequestId { get; }
    DateTimeOffset RequestedOn { get; }
}