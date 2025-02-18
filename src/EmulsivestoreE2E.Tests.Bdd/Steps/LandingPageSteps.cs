using EmulsiveStoreE2E.Core.UiComponents;
using Reqnroll;
 
 namespace EmulsiveStoreE2E.Tests.Bdd.Steps;
 
 [Binding]
 internal class LandingPageSteps(ILandingPage landingPage)
 {
     [Then(@"the intro (title|description) will be (.*)")]
     public async Task VerifyIntroTitle(string contentType, string expectedText)
     {
         var (title, description) = await landingPage.GetIntroContentAsync();

         Assert.That(contentType == "title" ? title : description, Is.EqualTo(expectedText));
     }
 }