using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using Reqnroll;
using Serilog;

namespace EmulsiveStoreE2E.Tests.Bdd.Steps;

[Binding]
public class NavigationStepDefs(ILandingPage landingPage, ILogger logger)
{
    [Given(@"am on the Emulsive Store {StoreSection} page")]
    [Given(@"am on the {StoreSection} page")]
    public async Task BrowseToLandingPage(StoreSection section)
    {
        logger.Information("Landing page: {@Hashcode}", landingPage.GetHashCode());
        switch (section)
        {
            case StoreSection.Landing:
                await landingPage.NavigateToStoreAsync();
                break;
        }
    }
}