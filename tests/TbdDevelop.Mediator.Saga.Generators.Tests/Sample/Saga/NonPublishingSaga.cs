using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class NonPublishingSaga
    : Saga<SampleSagaState>,
        IAmStartedBy<SampleNotification>
{
    public bool HandlerWasCalled { get; private set; }

    public override bool IsComplete => HandlerWasCalled;

    public ValueTask Handle(SampleNotification @event)
    {
        HandlerWasCalled = true;

        return ValueTask.CompletedTask;
    }
}