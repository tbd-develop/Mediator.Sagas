using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas;

public abstract class Saga<TState>(Guid orchestrationIdentifier) : ISaga<TState>
    where TState : class, new()
{
    public virtual bool IsComplete => false;
    public virtual TimeSpan? Expires => null; 
    public Guid OrchestrationIdentifier { get; } = orchestrationIdentifier;
    object ISaga.State => State;

    private TState? _state;
    public TState State => _state ??= new TState();

    void ISaga.ApplyState(object state) => ApplyState((TState)state);
    public void ApplyState(TState state) => _state = state;
}