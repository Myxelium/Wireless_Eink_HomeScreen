using MediatR;

namespace HomeApi.Extensions;

public static class MediatorExtensions
{
    public static async Task<T?> TrySendAsync<T>(
        this IMediator mediator,
        IRequest<T> request,
        CancellationToken cancellationToken) where T : class
    {
        try
        {
            return await mediator.Send(request, cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch
        {
            return null;
        }
    }
}