using Microsoft.Extensions.DependencyInjection;

namespace TbdDevelop.Mediator.Sagas.Configuration;

public static class ServiceCollectionExtensions
{
     public static IServiceCollection AddSagas(this IServiceCollection serviceCollection, 
          Action<SagaConfiguration> configure)
     {
          var configuration = new SagaConfiguration(serviceCollection);

          configure(configuration);

          return serviceCollection;
     }
}