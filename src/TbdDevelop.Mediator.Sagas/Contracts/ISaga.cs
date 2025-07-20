namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISaga
{
    bool IsComplete { get; }
    Guid OrchestrationIdentifier { get; }
    int MaximumTriggerCount { get; set; }
    TimeSpan? NextTriggerTime { get; set; }
    TimeSpan? TriggerInterval { get; set; }
    DateTime? LastTriggered { get; set; }
    object State { get; }
    Task OnTrigger(CancellationToken cancellationToken);

    void ApplyState(object state);
}

public interface ISaga<TState> : ISaga
    where TState : class, new()
{
    new TState State { get; }

    void ApplyState(TState state);
}