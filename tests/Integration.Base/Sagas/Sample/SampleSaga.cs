using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace Integration.Base.Sagas.Sample;

public class SampleSaga(Guid orchestrationIdentifier)
    : Saga<SimpleState>(orchestrationIdentifier),
        IHandle<SampleNotification>
{
    public void Handle(SampleNotification @event)
    {
        State.Value = $"{@event.Id}";
    }
}

public class SimpleState
{
    public string Value { get; set; } = null!;
}