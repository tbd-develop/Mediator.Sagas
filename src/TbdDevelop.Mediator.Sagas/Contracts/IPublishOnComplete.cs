using Mediator;

namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface IPublishOnComplete
{
    INotification Publish();
}

public interface IPublishOnComplete<out TNotification> : IPublishOnComplete
    where TNotification : INotification
{
    new TNotification Publish();
}