using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class StartedSaga : Saga<SampleSagaState>,
    IAmStartedBy<SampleNotification>
{
    public bool HandlerWasCalled { get; private set; }
    public SampleNotification NotificationWas { get; private set; } = null!;

    public StartedSaga(Guid orchestrationIdentifier) : base(orchestrationIdentifier)
    {
    }

    public override bool IsComplete { get; }

    public void Handle(SampleNotification @event)
    {
        HandlerWasCalled = true;
        NotificationWas = @event;
        State.Data = new object[] { @event };
    }
}