namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISaga
{
    bool IsComplete { get; }
    Guid OrchestrationIdentifier { get; }
    DateTime? NextTriggerTime { get; set; }
    TimeSpan? TriggerInterval { get; set; }
    DateTime? LastTriggered { get; set; }
    object State { get; }
    Task Trigger(CancellationToken cancellationToken);

    void ApplyState(Guid orchestrationIdentifier, object state);
}

public interface ISaga<TState> : ISaga
    where TState : class, new()
{
    new TState State { get; }

    void ApplyState(Guid orchestrationIdentifier, TState state);
}