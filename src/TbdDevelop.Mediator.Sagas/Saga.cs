using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas;

public abstract class Saga<TState> : ISaga<TState>
    where TState : class, new()
{
    public virtual bool IsComplete => false;

    public virtual DateTime? NextTriggerTime { get; set; }

    public virtual TimeSpan? TriggerInterval { get; set; }

    public DateTime? LastTriggered { get; set; }
    public bool HasTrigger => NextTriggerTime.HasValue || TriggerInterval.HasValue;

    private Guid _orchestrationIdentifier;
    public Guid OrchestrationIdentifier => _orchestrationIdentifier;

    public int Version { get; set; }
    object ISaga.State => State;

    private TState? _state;
    public TState State => _state ??= new TState();

    void ISaga.ApplyState(Guid orchestrationIdentifier, object state) =>
        ApplyState(orchestrationIdentifier, (TState)state);

    public void ApplyState(Guid orchestrationIdentifier, TState state)
    {
        _orchestrationIdentifier = orchestrationIdentifier;
        _state = state;
    }

    public virtual Task TriggerImpl(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Trigger(CancellationToken cancellationToken)
    {
        if (!HasTrigger)
        {
            return;
        }

        LastTriggered = DateTime.UtcNow;
        NextTriggerTime = DateTime.UtcNow.Add(TriggerInterval!.Value);

        await TriggerImpl(cancellationToken);
    }
}