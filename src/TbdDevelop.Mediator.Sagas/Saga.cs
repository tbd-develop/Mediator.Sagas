using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas;

public abstract class Saga<TState>(Guid orchestrationIdentifier) : ISaga<TState>
    where TState : class, new()
{
    public abstract bool IsComplete { get; }
    public Guid OrchestrationIdentifier { get; } = orchestrationIdentifier;
    object ISaga.State { get; } = null!;
    public TState State { get; private set; } = new();

    public void ApplyState(TState state) => State = state;
}