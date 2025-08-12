using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class StartedSaga
    : Saga<SampleSagaState>,
        IAmStartedBy<SampleNotification>
{
    public bool HandlerWasCalled { get; private set; }
    public SampleNotification NotificationWas { get; private set; } = null!;

    public ValueTask Handle(SampleNotification @event)
    {
        HandlerWasCalled = true;
        NotificationWas = @event;
        State.Data = [@event];

        return ValueTask.CompletedTask;
    }
}