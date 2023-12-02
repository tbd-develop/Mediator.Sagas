namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISaga
{
    bool IsComplete { get; }
    Guid OrchestrationIdentifier { get; }
    object State { get; }
    
    void ApplyState(object state);
}

public interface ISaga<TState> : ISaga
    where TState : class, new()
{
    new TState State { get; }

    void ApplyState(TState state);
}