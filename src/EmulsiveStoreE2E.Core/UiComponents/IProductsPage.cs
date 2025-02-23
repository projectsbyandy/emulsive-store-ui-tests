namespace EmulsiveStoreE2E.Core.UiComponents;

public interface IProductsPage : IWithProductFilter
{
    Task<Dictionary<string, bool>> IsKeywordPresentInAllProductAsync(string keyword);
}