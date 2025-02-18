using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using Reqnroll;
using Ardalis.GuardClauses;

namespace EmulsiveStoreE2E.Tests.Bdd.Steps;

[Binding]
internal class FilterStepDefinitions(IProductsPage productsPage)
{
    [Given("Filter on the following options:")]
    public async Task GivenFilterOnTheFollowingOptions(Table table)
    {
        var filterOptions = table.CreateSet<SetFilterOptions>();
        foreach (var option in filterOptions)
        {
            await productsPage.SetFilterOptionAsync(option.FilterOption, Guard.Against.Null(option.Value));
            await productsPage.SearchAsync();
        }
    }
    
    [When("I click on Reset")]
    public async Task WhenIClickOnReset()
    {
        await productsPage.ResetFilterAsync();
    }
    
    [Then("the filters will revert back to default")]
    public async Task VerifyFiltersRevertedToDefault()
    {
        var keyword = await productsPage.GetFilterValueAsync(FilterOption.Keyword);
        Assert.That(keyword, Is.EqualTo(string.Empty));
        
        var format = await productsPage.GetFilterValueAsync(FilterOption.Format);
        Assert.That(format, Is.EqualTo("all"));

        var manufacturer = await productsPage.GetFilterValueAsync(FilterOption.Manufacturer);
        Assert.That(manufacturer, Is.EqualTo("all"));
        
        var orderby = await productsPage.GetFilterValueAsync(FilterOption.OrderBy);
        Assert.That(orderby, Is.EqualTo("a-z"));
        
        var onsale = await productsPage.GetFilterValueAsync(FilterOption.OnSale);
        Assert.That(onsale, Is.EqualTo("unchecked"));
        
        var price = await productsPage.GetFilterValueAsync(FilterOption.Price);
        Assert.That(price, Is.EqualTo("3000"));
    }

    private record SetFilterOptions
    {
        public FilterOption FilterOption { get; init; }
        public string? Value { get; init; }
    }
}