namespace Trigger.Integration.Tests.Sagas.State;

public class SampleState
{
    public bool HasBeenTriggered { get; set; } = false;
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
}