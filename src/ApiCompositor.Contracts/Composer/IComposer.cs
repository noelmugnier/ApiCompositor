namespace ApiCompositor.Contracts.Composer;

public interface IComposer{
    string RequestId { get; }
    DateTimeOffset RequestedOn { get; }
}