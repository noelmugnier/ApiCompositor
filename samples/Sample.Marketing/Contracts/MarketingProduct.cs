namespace Sample.Marketing;

public record MarketingProduct(Guid Id, string Name)
{
    public string? Description { get; init; }
}