using Mediator;

namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IPublishOnComplete<out TNotification>
    where TNotification : INotification
{
    TNotification Publish();
}