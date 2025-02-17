using Reqnroll.Autofac;
using EmulsiveStoreE2E.Tests.Bdd.Ioc;
using EmulsiveStoreE2E.Tests.Bdd.Steps;
using Reqnroll.Autofac.ReqnrollPlugin;
using ContainerBuilder = Autofac.ContainerBuilder;
using Reqnroll;

namespace EmulsiveStoreE2E.Tests.Bdd;

[Binding]
internal class DependencyRegistration
{
    [ScenarioDependencies]
    public static void RegisterScenarioDependencies(ContainerBuilder builder)
    {
        // Register scenario scoped dependencies
        builder
            .AddConfigSupport()
            .AddResilienceSupport()
            .AddUiServices()
            .AddPlaywrightComponents();
        
        builder.AddReqnrollBindings<LandingPageSteps>();
        Console.WriteLine($"Container hash {builder.GetHashCode()}");
    }
}
