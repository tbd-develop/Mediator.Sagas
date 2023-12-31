﻿namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISagaPersistence
{
    Task<TSaga?> FetchSagaIfExistsByOrchestrationId<TSaga>(Guid identifier,
        CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    Task Save<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga;
    Task Delete<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga;
}