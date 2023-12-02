namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IHandle<in TNotification>
    where TNotification : class, IOrchestratedNotification
{
    void Handle(TNotification @event);
}