using Autofac;
using EmulsiveStoreE2E.Core.Services;
using EmulsiveStoreE2E.Tests.Bdd.Ioc;
using EmulsiveStoreE2E.Ui.Services;
using Reqnroll;

namespace EmulsiveStoreE2E.Tests.Bdd.Hooks;

[Binding]
internal class SetupHooks
{
    [BeforeTestRun]
    public static async Task HeathCheck()
    {
        var builder = new ContainerBuilder();
        builder
            .AddConfigSupport()
            .AddLoggingSupport()
            .AddPlaywrightComponents();
        
        builder.RegisterType< HealthCheck>().As<IHeathCheck>().SingleInstance();
        
        await builder.Build().Resolve<IHeathCheck>().VerifyEmulsiveStoreRunningAsync();
    }
}