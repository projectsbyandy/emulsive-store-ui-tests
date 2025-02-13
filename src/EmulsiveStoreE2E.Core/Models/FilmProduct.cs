using Ardalis.GuardClauses;

namespace EmulsiveStoreE2E.Core.Models;

public record FilmProduct
{
    public int Id { get; set; }
    public required string Name { get; init; }
    public required string PriceWithCurrency { get; init; }

    private double _price;
    public double Price
    {
        get => _price is 0 ? RemoveCurrency(PriceWithCurrency) : _price;
        set => _price = value;
    }

    public required string ImageUrl { get; set; }
    public string? Iso { get; set; }
    public string? Format { get; set; }

    private double RemoveCurrency(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, "PriceWithCurrency has not been set");
        return Convert.ToDouble(value.Substring(1, value.Length-1));
    }
}