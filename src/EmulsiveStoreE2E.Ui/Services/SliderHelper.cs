using Ardalis.GuardClauses;
using Microsoft.Playwright;

namespace EmulsiveStoreE2E.Ui.Services;

internal class SliderHelper
{
    public static async Task SetAsync(ILocator slider, IPage page, float maxValue, string value)
    {
        var percentage = (float.Parse(value) / maxValue) * 100f;

        var box = Guard.Against.Null(await slider.BoundingBoxAsync());
        var targetX = box.X + (box.Width * (percentage / 100f));
        await page.Mouse.MoveAsync((float)(box.X + box.Width / 2), (float)(box.Y + box.Height / 2));
        await page.Mouse.DownAsync();
                    
        await page.Mouse.MoveAsync(targetX, (float)(box.Y + box.Height / 2));
        await page.Mouse.UpAsync();
    }
    
}