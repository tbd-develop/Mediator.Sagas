using System.Collections.Concurrent;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas;

/// <summary>
/// In Memory implementation for Saga Persistence. Useful in testing or in scenarios where it
/// doesn't matter if sagas survive an application restart.
/// </summary>
public class InMemorySagaPersistence : ISagaPersistence
{
    private readonly ConcurrentDictionary<Guid, ISaga> _sagas = new();

    public Task<TSaga?> FetchSagaIfExistsByOrchestrationId<TSaga>(Guid identifier,
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

    public Task Save<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(saga);

        _sagas.AddOrUpdate(saga.OrchestrationIdentifier, saga, (_, _) => saga);

        return Task.CompletedTask;
    }

    public Task Delete<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(saga);

        _sagas.TryRemove(saga.OrchestrationIdentifier, out _);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<ISaga>> AllSagas(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_sagas.Values.AsEnumerable());
    }
}