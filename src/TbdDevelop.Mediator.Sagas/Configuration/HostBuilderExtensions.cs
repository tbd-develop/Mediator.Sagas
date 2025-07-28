using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TbdDevelop.Mediator.Sagas.Services;

namespace TbdDevelop.Mediator.Sagas.Configuration;

public static class HostBuilderExtensions
{
    /// <summary>
    /// Enable Sagas behavior, including the service monitoring for triggers
    /// </summary>
    /// <param name="builder">Host Builder from application</param>
    /// <param name="configure">Further configure saga behavior, including enabling persistence</param>
    /// <typeparam name="THostBuilder"></typeparam>
    /// <returns></returns>
    public static THostBuilder AddSagas<THostBuilder>(this THostBuilder builder,
        Action<SagaConfiguration> configure)
        where THostBuilder : IHostApplicationBuilder
    {
        var configuration = new SagaConfiguration(builder.Services);

        configure(configuration);

        builder.Services.Configure<TriggerOptions>(builder.Configuration.GetSection("sagas:triggers"));
        builder.Services.AddHostedService<SagaTriggerService>();

        return builder;
    }
}