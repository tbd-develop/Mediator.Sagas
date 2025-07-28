namespace TbdDevelop.Mediator.Sagas.Contracts;

public interface ISagaFactory
{
    IServiceProvider Provider { get; }
    TSaga CreateSaga<TSaga>(Guid orchestrationIdentifier, object? state = null) where TSaga : class, ISaga;
}