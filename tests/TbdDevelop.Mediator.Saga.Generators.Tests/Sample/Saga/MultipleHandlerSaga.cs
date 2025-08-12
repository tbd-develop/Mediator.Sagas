using TbdDevelop.Mediator.Sagas;

namespace TbdDevelop.Mediator.Saga.Generators.Tests.Sample.Saga;

public class MultipleHandlerSaga
    : Saga<MultipleHandlerSagaState>,
        IHandle<SampleNotification>,
        IHandle<FurtherNotification>
{
    public ValueTask Handle(SampleNotification @event)
    {
        throw new NotImplementedException();
    }

    public ValueTask Handle(FurtherNotification @event)
    {
        throw new NotImplementedException();
    }
}