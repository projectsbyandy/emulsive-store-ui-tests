using Ardalis.GuardClauses;
using Microsoft.Playwright;

namespace EmulsiveStoreE2E.Ui.Services;

public static class LocatorExtensions
{
    public static ILocator TraverseUpwards(this ILocator locator, int times)
    {
        ILocator? parent = null;
        
        for (var i = 0; i < times; i++)
        {
            parent = locator.Locator("..");
        }

        return Guard.Against.Null(parent);
    }
}