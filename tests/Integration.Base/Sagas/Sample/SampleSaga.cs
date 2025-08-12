using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace Integration.Base.Sagas.Sample;

public class SampleSaga
    : Saga<SimpleState>,
        IHandle<SampleNotification>
{
    public ValueTask Handle(SampleNotification @event)
    {
        State.Value = $"{@event.Id}";

        return ValueTask.CompletedTask;
    }
}

public class SimpleState
{
    public string Value { get; set; } = null!;
}