using EmulsiveStoreE2E.Core.Models;

namespace EmulsiveStoreE2E.Core.UiComponents;

public interface IHeader
{
    public Task NavigateTo(StoreSection section);
}