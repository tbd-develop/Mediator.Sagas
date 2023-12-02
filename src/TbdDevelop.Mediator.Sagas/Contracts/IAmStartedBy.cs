namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IAmStartedBy<in TNotification> : IHandle<TNotification>
    where TNotification : class, IOrchestratedNotification
{
}