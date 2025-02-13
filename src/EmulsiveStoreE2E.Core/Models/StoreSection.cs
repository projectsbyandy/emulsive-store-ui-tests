using System.ComponentModel;

namespace EmulsiveStoreE2E.Core.Models;

public enum StoreSection
{
    [Description("/about")]
    About,
    [Description("/products")]
    Products,
    [Description("/cart")]
    Cart, 
    [Description("/checkout")]
    Checkout, 
    [Description("/orders")]
    Orders
}