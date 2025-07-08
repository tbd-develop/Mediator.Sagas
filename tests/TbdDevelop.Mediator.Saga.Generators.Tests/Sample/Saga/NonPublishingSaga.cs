using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class NonPublishingSaga(Guid orchestrationIdentifier)
    : Saga<SampleSagaState>(orchestrationIdentifier),
        IAmStartedBy<SampleNotification>
{
    public bool HandlerWasCalled { get; private set; }

    public override bool IsComplete => HandlerWasCalled;

    public void Handle(SampleNotification @event)
    {
        HandlerWasCalled = true;
    }
}