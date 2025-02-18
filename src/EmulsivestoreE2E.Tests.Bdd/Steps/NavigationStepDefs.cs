using EmulsiveStoreE2E.Core.Models;
using EmulsiveStoreE2E.Core.UiComponents;
using Reqnroll;
using Serilog;

namespace EmulsiveStoreE2E.Tests.Bdd.Steps;

[Binding]
internal class NavigationStepDefs(INavigate navigate, ILogger logger)
{
    [Given(@"am on the Emulsive Store {StoreSection} page")]
    [Given(@"am on the {StoreSection} page")]
    public async Task BrowseToLandingPage(StoreSection section)
    {
        logger.Information("Navigate hash: {@Hashcode}", navigate.GetHashCode());
        await navigate.To(section);
    }
}