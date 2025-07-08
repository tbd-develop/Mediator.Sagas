using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class MultipleHandlerSaga(Guid orchestrationIdentifier)
    : Saga<MultipleHandlerSagaState>(orchestrationIdentifier),
        IHandle<SampleNotification>,
        IHandle<FurtherNotification>
{
    public override bool IsComplete { get; }

    public void Handle(SampleNotification @event)
    {
        throw new NotImplementedException();
    }

    public void Handle(FurtherNotification @event)
    {
        throw new NotImplementedException();
    }
}