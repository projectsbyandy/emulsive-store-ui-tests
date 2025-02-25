namespace EmulsiveStoreE2E.Core.Services;

public interface IHeathCheck
{
    public Task VerifyEmulsiveStoreRunningAsync();
}