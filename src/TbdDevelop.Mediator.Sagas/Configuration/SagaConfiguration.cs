using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas.Configuration;

public class SagaConfiguration
{
    private readonly IServiceCollection _serviceCollection;

    public SagaConfiguration(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public SagaConfiguration RegisterSaga<TSaga>()
        where TSaga : class, ISaga
    {
        _serviceCollection.AddTransient<TSaga>();

        return this;
    }

    public SagaConfiguration RegisterComponent(Action<IServiceCollection> register)
    {
        register(_serviceCollection);

        return this;
    }
}