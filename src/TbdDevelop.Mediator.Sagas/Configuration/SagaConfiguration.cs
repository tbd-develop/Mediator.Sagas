using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas.Configuration;

/// <summary>
/// Does a thing
/// </summary>
/// <param name="serviceCollection"></param>
public class SagaConfiguration(IServiceCollection serviceCollection)
{
    public SagaConfiguration RegisterSaga<TSaga>()
        where TSaga : class, ISaga
    {
        serviceCollection.AddScoped<TSaga>();

        return this;
    }

    public SagaConfiguration UseInMemoryPersistence()
    {
        serviceCollection.AddSingleton<ISagaPersistence, InMemorySagaPersistence>();

        return this;
    }

    public SagaConfiguration RegisterComponent(Action<IServiceCollection> register)
    {
        register(serviceCollection);

        return this;
    }
}