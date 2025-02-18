using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using Microsoft.Playwright;
using Ardalis.GuardClauses;

namespace EmulsiveStoreE2E.Ui.UiComponents;

public abstract class WithProductFilter(IPage page) : IWithProductFilter
{
    public Task<bool> IsOptionPresentInFilterAsync(FilterOption filterOption)
    {
        throw new NotImplementedException();
    }

    public async Task SetFilterOptionAsync(FilterOption filterOption, string value)
    {
        Dictionary<FilterOption, Func<Task>> optionSetDictionary = new()
        {
            { FilterOption.Keyword, async () => await page.Locator("input[type='keyword']").FillAsync(value) },
            { FilterOption.Format, async () => await page.Locator("select[name='format']").SelectOptionAsync(value) },
            { FilterOption.Manufacturer, async () => await page.Locator("select[name='manufacturer']").SelectOptionAsync(value) }
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
            { FilterOption.Keyword, async () => Guard.Against.Null(await page.Locator("input[type='keyword']").TextContentAsync()) },
            { FilterOption.Format, async () => Guard.Against.Null(await page.Locator("select[name='format']").InputValueAsync()) },
            { FilterOption.Manufacturer, async () => Guard.Against.Null(await page.Locator("select[name='manufacturer']").InputValueAsync()) }
        };

        optionGetDictionary.TryGetValue(filterOption, out var getFilterValue);
        return getFilterValue.Invoke();
    }

}