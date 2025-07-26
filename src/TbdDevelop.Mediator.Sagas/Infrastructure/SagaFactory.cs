using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas.Infrastructure;

public class SagaFactory(IServiceProvider serviceProvider) : ISagaFactory
{
    public IServiceProvider Provider { get; } = serviceProvider;

    public TSaga CreateSaga<TSaga>(Guid orchestrationIdentifier, object? state = null) where TSaga : class, ISaga
    {
        var saga = Provider.GetRequiredService<TSaga>();

        saga.ApplyState(orchestrationIdentifier, state ?? saga.State);

        return saga;
    }
}