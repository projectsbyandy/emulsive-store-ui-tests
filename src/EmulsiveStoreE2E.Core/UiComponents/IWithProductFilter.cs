using EmulsiveStoreE2E.Core.Models;

namespace EmulsiveStoreE2E.Core.UiComponents;

public interface IWithProductFilter
{
    Task<bool> IsOptionPresentInFilterAsync(FilterOption filterOption);
    Task SetFilterOptionAsync(FilterOption filterOption, string value);
    Task<string> GetFilterValueAsync(FilterOption filterOption);
    Task SearchAsync();
    Task ResetFilterAsync();
}