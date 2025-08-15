using MongoDb.Integration.Tests.Sagas.Commands;
using MongoDb.Integration.Tests.Sagas.State;
using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace MongoDb.Integration.Tests.Sagas;

public class SampleTriggerSaga : Saga<SampleState>,
    IAmStartedBy<SampleNotification>
{
    public override TimeSpan? TriggerInterval { get; set; } = TimeSpan.FromSeconds(5);
    public override DateTime? NextTriggerTime { get; set; } = DateTime.UtcNow.AddSeconds(5);

    public ValueTask Handle(SampleNotification @event, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    protected override Task TriggerImpl(CancellationToken cancellationToken)
    {
        State.HasBeenTriggered = true;

        return base.TriggerImpl(cancellationToken);
    }
}