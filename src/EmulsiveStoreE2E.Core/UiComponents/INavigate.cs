using EmulsiveStoreE2E.Core.Models;

namespace EmulsiveStoreE2E.Core.UiComponents;

public interface INavigate
{
    Task To(StoreSection storeSection);
}