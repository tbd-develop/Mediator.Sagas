namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IHandle<in TNotification>
    where TNotification : class, IOrchestratedNotification
{
    ValueTask Handle(TNotification @event, CancellationToken cancellationToken);
}