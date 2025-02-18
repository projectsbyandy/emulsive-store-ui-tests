using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using Reqnroll;

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
            await productsPage.SetFilterOptionAsync(option.FilterOption, option.Value);
        }
        
        await productsPage.SearchAsync();
    }
    
    [StepDefinition(@"I sort on the {FilterOption} with value {string}")]
    public async Task VerifyIntroTitle(FilterOption filterOption, string searchValue)
    {
        await productsPage.SetFilterOptionAsync(filterOption, searchValue);
        await productsPage.SearchAsync();
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
    }

    private record SetFilterOptions
    {
        public FilterOption FilterOption { get; init; }
        public string Value { get; init; }
    }
}