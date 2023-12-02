namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISagaPersistence
{
    TSaga Retrieve<TSaga, TState>(Guid identifier)
        where TSaga : Saga<TState>
        where TState : class, new();

    Task Save<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga;
}