namespace Sample.Marketing;

public record MarketingProduct(Guid Id, string Name, string Category)
{
    public string? Description { get; init; }
}