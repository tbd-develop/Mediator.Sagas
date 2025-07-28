using TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Notifications;
using TbdDevelop.Mediator.Saga.Generators.Tests.Sample.State;
using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class SampleSaga : Saga<SampleSagaState>,
    IHandle<SampleNotification>
{
    public bool HandlerWasCalled { get; private set; }
    public SampleNotification NotificationWas { get; private set; } = null!;

    public override bool IsComplete { get; } = false;

    public void Handle(SampleNotification @event)
    {
        HandlerWasCalled = true;
        NotificationWas = @event;
    }
}