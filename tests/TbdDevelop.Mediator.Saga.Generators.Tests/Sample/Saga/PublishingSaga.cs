using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class PublishingSaga(Guid orchestrationIdentifier) : Saga<SampleSagaState>(orchestrationIdentifier),
    IAmStartedBy<SampleNotification>,
    IPublishOnComplete<CompleteNotification>
{
    public bool MessageWasHandled { get; private set; }

    public override bool IsComplete => MessageWasHandled;

    public void Handle(SampleNotification @event)
    {
        MessageWasHandled = true;
    }

    public CompleteNotification Publish()
    {
        return new CompleteNotification();
    }

    INotification IPublishOnComplete.Publish()
    {
        return Publish();
    }
}