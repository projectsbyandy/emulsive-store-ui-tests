using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using Microsoft.Playwright;
using Ardalis.GuardClauses;

namespace EmulsiveStoreE2E.Ui.UiComponents;

public abstract class WithProductFilter(IPage page) : IWithProductFilter
{
    private const string KeywordLocator = "input[type='keyword']";
    private const string FormatLocator = "select[name='format']";
    private const string ManufacturerLocator = "select[name='manufacturer']";
    private const string OrderByLocator = "select[name='orderby']";
    private const string OnSaleLocator = "#onsale";
    
    public Task<bool> IsOptionPresentInFilterAsync(FilterOption filterOption)
    {
        throw new NotImplementedException();
    }

    public async Task SetFilterOptionAsync(FilterOption filterOption, string value)
    {
        Dictionary<FilterOption, Func<Task>> optionSetDictionary = new()
        {
            { FilterOption.Keyword, async () => await page.Locator(KeywordLocator).FillAsync(value) },
            { FilterOption.Format, async () => await page.Locator(FormatLocator).SelectOptionAsync(value) },
            { FilterOption.Manufacturer, async () => await page.Locator(ManufacturerLocator).SelectOptionAsync(value) },
            { FilterOption.OrderBy, async () => await page.Locator(OrderByLocator).SelectOptionAsync(value) },
            { FilterOption.OnSale, async () =>
                {
                    var onSaleElement = page.Locator(OnSaleLocator);
                    if (value == "checked" && await onSaleElement.IsCheckedAsync() == false)
                    {
                        await onSaleElement.ClickAsync();
                    }
                }
            }
        };
        
        optionSetDictionary.TryGetValue(filterOption, out var filterSet);
        await Guard.Against.Null(filterSet).Invoke();
    }

    public async Task SearchAsync()
        => await page.Locator("button[type='submit']").ClickAsync();

    public async Task ResetFilterAsync()
        => await page.GetByTestId("reset").ClickAsync();

    public Task<string> GetFilterValueAsync(FilterOption filterOption)
    {
        Dictionary<FilterOption, Func<Task<string>>> optionGetDictionary = new()
        {
            { FilterOption.Keyword, async () => Guard.Against.Null(await page.Locator(KeywordLocator).TextContentAsync()) },
            { FilterOption.Format, async () => Guard.Against.Null(await page.Locator(FormatLocator).InputValueAsync()) },
            { FilterOption.Manufacturer, async () => Guard.Against.Null(await page.Locator(ManufacturerLocator).InputValueAsync()) },
            { FilterOption.OrderBy, async () => Guard.Against.Null(await page.Locator(OrderByLocator).InputValueAsync()) },
            { FilterOption.OnSale, async () => Guard.Against.Null(await page.Locator(OnSaleLocator).IsCheckedAsync() ? "checked" : "unchecked") },

        };

        optionGetDictionary.TryGetValue(filterOption, out var getFilterValue);
        return Guard.Against.Null(getFilterValue).Invoke();
    }

}