using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using Reqnroll;
using Ardalis.GuardClauses;
using EmulsiveStoreE2E.Core.Exceptions;

namespace EmulsiveStoreE2E.Tests.Bdd.Steps;

[Binding]
internal class FilterStepDefinitions(IProductsPage productsPage)
{
    private IEnumerable<FilterOptions>? _transientFilterOptions;
    
    [StepDefinition("Filtering/Filter on the following options:")]
    public async Task SetTheFilterOptions(Table table)
    {
        _transientFilterOptions = table.CreateSet<FilterOptions>();
        foreach (var option in _transientFilterOptions)
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
    
    [Then("the values will be persisted in the filter options")]
    public async Task ThenTheValuesWillBePersistedInTheFilterOptions()
    {
        if (_transientFilterOptions is null)
            throw new RegressionTestException("Unable to Verify as Filter Options have not been set");

        foreach (var transientFilterOption in _transientFilterOptions)
        {
            var actualValue = await productsPage.GetFilterValueAsync(transientFilterOption.FilterOption);
            Assert.That(actualValue, Is.EqualTo(transientFilterOption.Value));
        }
    }

    [Then("all filtered products will contain the keyword in either the title, description or manufacturer")]
    public async Task ProductsContainKeyWordOnPage()
    {
        ArgumentNullException.ThrowIfNull(_transientFilterOptions, "Filter options have not been set");
        
        var keyword = Guard.Against.Null(_transientFilterOptions.ToList().Find(o => o.FilterOption == FilterOption.Keyword)?.Value);
        
        var filterOutcome = await productsPage.IsKeywordPresentInAllProductAsync(keyword);
        
        foreach (var film in filterOutcome)
        {
            Assert.That(film.Value, Is.EqualTo(true), $"{film.Key}: Keyword not found in name, manufacturer or description");
        }
    }
    
    private record FilterOptions
    {
        public FilterOption FilterOption { get; init; }
        public string? Value { get; init; }
    }
}