using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas;

public abstract class Saga<TState>(Guid orchestrationIdentifier) : ISaga<TState>
    where TState : class, new()
{
    public virtual bool IsComplete => false;

    public virtual int MaximumTriggerCount { get; set; }

    public virtual TimeSpan? NextTriggerTime { get; set; }

    public virtual TimeSpan? TriggerInterval { get; set; }
    
    public DateTime? LastTriggered { get; set; }
    public bool HasTrigger => NextTriggerTime.HasValue || TriggerInterval.HasValue;
    public Guid OrchestrationIdentifier { get; } = orchestrationIdentifier;
    object ISaga.State => State;

    private TState? _state;
    public TState State => _state ??= new TState();

    void ISaga.ApplyState(object state) => ApplyState((TState)state);
    public void ApplyState(TState state) => _state = state;

    public virtual Task OnTrigger(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}