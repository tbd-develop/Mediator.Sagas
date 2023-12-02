namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISagaPersistence
{
    Task<TSaga?> FetchSagaIfExistsByOrchestrationId<TSaga, TState>(Guid identifier, CancellationToken cancellationToken = default)
        where TSaga : Saga<TState>
        where TState : class, new();

    Task Save<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga;
}