using Mediator;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas.Infrastructure;

public abstract class SagaNotificationHandlerBase<TSaga, TNotification>(
    IMediator mediator,
    ISagaPersistence persistence)
    : INotificationHandler<TNotification>
    where TSaga : class, ISaga
    where TNotification : INotification
{
    private const int MaximumRetries = 3;

    protected abstract ValueTask<TSaga?> HandleNotification(
        TNotification notification,
        CancellationToken cancellationToken);

    public async ValueTask Handle(TNotification notification, CancellationToken cancellationToken)
    {
        if (notification is not IOrchestratedNotification)
        {
            return;
        }

        int attempt = 0;

        while (attempt < MaximumRetries)
        {
            var saga = await HandleNotification(notification, cancellationToken);

            if (saga is null)
            {
                break;
            }

            if (saga.IsComplete)
            {
                if (saga is IPublishOnComplete publisher)
                {
                    await mediator.Publish(publisher.Publish(), cancellationToken);
                }

                await persistence.Delete(saga, cancellationToken);

                break;
            }

            var success = await persistence.UpdateIfVersionMatches(saga, cancellationToken);

            if (success)
            {
                break;
            }

            attempt++;
        }

        if (attempt == MaximumRetries)
        {
            throw new SagaUpdatedFailedException(
                $"{typeof(TNotification).FullName} failed to handle {typeof(TSaga).FullName})");
        }
    }
}