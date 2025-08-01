using System.Collections.Concurrent;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas;

/// <summary>
/// In Memory implementation for Saga Persistence. Useful in testing or
/// in scenarios where it doesn't matter if sagas survive an application restart.
///
/// NOT suitable for distributed components
/// </summary>
public class InMemorySagaPersistence : ISagaPersistence
{
    private readonly ConcurrentDictionary<Guid, ISaga> _sagas = new();

    public Task<TSaga?> FetchSagaByOrchestrationIdentifier<TSaga>(Guid identifier,
        CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_sagas.TryGetValue(identifier, out var saga) && saga is TSaga typedSaga)
        {
            return Task.FromResult<TSaga?>(typedSaga);
        }

        return Task.FromResult<TSaga?>(null);
    }

    public Task<bool> UpdateIfVersionMatches<T>(T saga, CancellationToken cancellationToken = default)
        where T : class, ISaga
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(saga);

        _sagas.AddOrUpdate(saga.OrchestrationIdentifier, saga, (_, _) => saga);

        return Task.FromResult(true);
    }

    public Task Delete<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(saga);

        _sagas.TryRemove(saga.OrchestrationIdentifier, out _);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<ISaga>> AllSagasToTrigger(int withinMs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = DateTime.UtcNow;
        var msBeforeNow = now.Subtract(TimeSpan.FromMilliseconds(withinMs));

        var results = _sagas.Values.Where(e =>
            (e is { NextTriggerTime: not null, LastTriggered: null } && e.NextTriggerTime <= msBeforeNow) ||
            (e is { NextTriggerTime: not null, LastTriggered: not null } && e.NextTriggerTime >= msBeforeNow &&
             e.NextTriggerTime <= now && e.LastTriggered <= msBeforeNow)
        ).AsEnumerable();

        return Task.FromResult(results);
    }
}