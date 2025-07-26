using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class MultipleHandlerSaga
    : Saga<MultipleHandlerSagaState>,
        IHandle<SampleNotification>,
        IHandle<FurtherNotification>
{
    public void Handle(SampleNotification @event)
    {
        throw new NotImplementedException();
    }

    public void Handle(FurtherNotification @event)
    {
        throw new NotImplementedException();
    }
}