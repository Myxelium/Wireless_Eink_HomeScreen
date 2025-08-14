namespace HomeApi.Extensions;

public static class ServiceCallExtensions
{
    public static async Task<TResult?> TryCallAsync<TService, TResult>(
        this TService service,
        Func<TService, Task<TResult>> action,
        ILogger logger,
        string errorMessage)
        where TResult : class?
    {
        try
        {
            return await action(service);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception exception)
        {
            logger.LogError(exception, errorMessage);
            return null;
        }
    }
}