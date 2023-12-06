using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class NonPublishingSaga : Saga<SampleSagaState>,
    IAmStartedBy<SampleNotification>
{
    public bool HandlerWasCalled { get; private set; }

    public NonPublishingSaga(Guid orchestrationIdentifier) : base(orchestrationIdentifier)
    {
    }

    public override bool IsComplete => HandlerWasCalled;

    public void Handle(SampleNotification @event)
    {
        HandlerWasCalled = true;
    }
}