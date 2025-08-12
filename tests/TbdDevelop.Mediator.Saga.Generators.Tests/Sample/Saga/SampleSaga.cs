using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class SampleSaga : Saga<SampleSagaState>,
    IHandle<SampleNotification>
{
    public bool HandlerWasCalled { get; private set; }
    public SampleNotification NotificationWas { get; private set; } = null!;

    public override bool IsComplete { get; } = false;

    public ValueTask Handle(SampleNotification @event)
    {
        HandlerWasCalled = true;
        NotificationWas = @event;

        return ValueTask.CompletedTask;
    }
}