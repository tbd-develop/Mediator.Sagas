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
    object ISaga.State => State;

    private TState? _state;
    public TState State => _state ??= new TState();

    void ISaga.ApplyState(object state) => ApplyState((TState)state);
    public void ApplyState(TState state) => _state = state;
}