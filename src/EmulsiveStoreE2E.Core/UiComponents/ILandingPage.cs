using EmulsiveStoreE2E.Core.Models;

namespace EmulsiveStoreE2E.Core.UiComponents;

public interface ILandingPage
{
    public Task<(string, string)> GetIntroContentAsync();
    public Task<IList<FilmProduct>> ExtractFeaturedFilmsAsync(int numberToExtract);
    public Task SelectOurProductsAsync();
    public Task SelectFeaturedProductAsync(string productName);
}