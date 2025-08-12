using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class PublishingSaga : Saga<SampleSagaState>,
    IAmStartedBy<SampleNotification>,
    IPublishOnComplete<CompleteNotification>
{
    public bool MessageWasHandled { get; private set; }

    public override bool IsComplete => MessageWasHandled;

    public ValueTask Handle(SampleNotification @event)
    {
        MessageWasHandled = true;

        return ValueTask.CompletedTask;
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