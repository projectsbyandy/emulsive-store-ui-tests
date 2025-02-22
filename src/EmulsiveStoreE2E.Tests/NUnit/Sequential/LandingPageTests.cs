using Ardalis.GuardClauses;
using EmulsiveStoreE2E.Core.Helpers;
using EmulsiveStoreE2E.Core.Helpers.RetryHelper;
using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using EmulsiveStoreE2E.Tests.NUnit.Sequential.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Serilog;

namespace EmulsiveStoreE2E.Tests.NUnit.Sequential;

/*
 * Example of Re-using the same IPage across tests
 */

internal class LandingPageTests : TestSetup
{
    private ILandingPage _landingPage;
    private INavigate _navigate;
    private IPage _page;
    private IResilienceRetry _resilienceRetry;

    [SetUp]
    public async Task Setup()
    {
        _landingPage = Guard.Against.Null(TestServices.GetService<ILandingPage>());
        _navigate = Guard.Against.Null(TestServices.GetService<INavigate>());
        _page = Guard.Against.Null(TestServices.GetService<IPage>());
        Guard.Against.Null(TestServices.GetService<ILogger>())
            .Information("Page hash from sequential tests: {@Hash}", _page.GetHashCode());
        
        _resilienceRetry = Guard.Against.Null(TestServices.GetService<IResilienceRetry>());
        
        await _navigate.To(StoreSection.Landing);
    }
    
    [Test]
    public async Task Verify_Landing_Page_Intro_Content()
    {
        // Arrange / Act
        var (introTitle, introContent) = await _landingPage.GetIntroContentAsync();
        
        // Assert
        Assert.That(introTitle, Is.EqualTo("Bringing film emulsive love!"));
        Assert.That(introContent, Is.EqualTo("We’re passionate about supporting photographers - big and small - and know what it means to have a shop you can trust and rely on."));
    }
    
    [Test, TestCaseSource(nameof(ExpectedFeaturedProducts))]
    public async Task Verify_Feature_Film_Content(FilmProduct expectedFilmProduct)
    {
        // Arrange / Act
        var products = await _landingPage.ExtractFeaturedFilmsAsync(5);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(products.Count, Is.EqualTo(5));
            CollectionAssert.Contains(products, expectedFilmProduct);
        });
    }

    static IEnumerable<FilmProduct> ExpectedFeaturedProducts()
    {
        yield return new FilmProduct { Id = 19, Name = "Gold 200", ImageUrl = "https://iili.io/2iqmyF9.webp", PriceWithCurrency = "£9.50" };
        yield return new FilmProduct { Id = 6, Name = "Hp5 Plus", ImageUrl = "https://iili.io/2iqmbGS.webp", PriceWithCurrency = "£7.35" };
        yield return new FilmProduct { Id = 24, Name = "800T", ImageUrl = "https://iili.io/2iqmiyG.webp", PriceWithCurrency = "£19.00" };
        yield return new FilmProduct { Id = 25, Name = "Rollei Infrared", ImageUrl = "https://iili.io/2iqm43X.webp", PriceWithCurrency = "£19.00" };
        yield return new FilmProduct { Id = 7, Name = "Portra 400", ImageUrl = "https://iili.io/2iqmm67.webp", PriceWithCurrency = "£12.90" };
    }
    
    [Test]
    public async Task Verify_Show_Products_Takes_User_to_Products_Page()
    {
        // Act
        await _landingPage.SelectOurProductsAsync();
        
        // Assert
        _resilienceRetry.UntilTrue("Waiting for products page to load",
            () => _page.Url.Contains(StoreSection.Products.GetDescription()), TimeSpan.FromSeconds(1), 5);
        StringAssert.Contains(StoreSection.Products.GetDescription(), _page.Url);
    }

    [Test]
    public async Task Verify_Selecting_a_Featured_Product_Takes_User_to_ProductDetails_Page()
    {
        await _landingPage.SelectFeaturedProductAsync("Gold 200");
    }
}