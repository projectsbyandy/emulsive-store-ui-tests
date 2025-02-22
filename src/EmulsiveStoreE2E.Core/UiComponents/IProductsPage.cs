using EmulsiveStoreE2E.Core.Models;

namespace EmulsiveStoreE2E.Core.UiComponents;

public interface IProductsPage : IWithProductFilter
{
    Task ExpectMoreThanOnePageAsync();
    Task<IList<FilmProduct>> GetProductsOnPageAsync(int pageNumber);
}