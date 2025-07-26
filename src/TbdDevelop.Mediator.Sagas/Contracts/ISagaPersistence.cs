namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISagaPersistence
{
    Task<TSaga?> FetchSagaByOrchestrationIdentifier<TSaga>(Guid identifier,
        CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    Task Save<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga;
    Task Delete<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga;
    Task<IEnumerable<ISaga>> AllSagasToTrigger(int withinMinutes, CancellationToken cancellationToken = default);
}