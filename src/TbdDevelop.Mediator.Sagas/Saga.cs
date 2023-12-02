using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas;

public abstract class Saga<TState> : ISaga<TState>
    where TState : class, new()
{
    protected Saga(Guid orchestrationIdentifier)
    {
        OrchestrationIdentifier = orchestrationIdentifier;
    }
    
    public abstract bool IsComplete { get; }
    public Guid OrchestrationIdentifier { get; }
    object ISaga.State { get; } = null!;
    public TState State { get; private set; }

    public void ApplyState(TState state) => State = state;
}