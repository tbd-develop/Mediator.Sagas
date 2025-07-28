using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;
using Trigger.Integration.Tests.Sagas.Commands;
using Trigger.Integration.Tests.Sagas.State;

namespace Trigger.Integration.Tests.Sagas;

public class SampleTriggerSaga : Saga<SampleState>,
    IAmStartedBy<SampleNotification>
{
    public override TimeSpan? TriggerInterval { get; set; } = TimeSpan.FromSeconds(5);
    public override DateTime? NextTriggerTime { get; set; } = DateTime.UtcNow.AddSeconds(5);

    public void Handle(SampleNotification @event)
    {
    }

    public override Task TriggerImpl(CancellationToken cancellationToken)
    {
        State.HasBeenTriggered = true;

        State.CancellationTokenSource.Cancel();

        return base.TriggerImpl(cancellationToken);
    }
}