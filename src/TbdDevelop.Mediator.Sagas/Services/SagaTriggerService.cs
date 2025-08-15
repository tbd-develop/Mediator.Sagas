using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas.Services;

public class SagaTriggerService(
    IServiceScopeFactory scopeFactory,
    IOptionsMonitor<TriggerOptions> options,
    ILogger<SagaTriggerService> logger) : BackgroundService
{
    private const int DefaultIntervalMs = 1000;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // every x minutes, we should check to see if we need to do anything with saga expiration? '
        // we have to check x minute, because the process to check the sagas, and the process inserting 
        // the sagas is not connected. Our other option is to constantly poll the 

        // Scale-out later needs to considered, sagas living across multiple instances of the servers 
        // can't handle 

        // It would be more efficient if I could leave this process dormant until a saga trigger is necessary, 
        // but we would need to shift that awake when a new saga was inserted. 

        return Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();

            while (!stoppingToken.IsCancellationRequested)
            {
                var stopWatch = Stopwatch.StartNew();
                
                var persistence = scope.ServiceProvider.GetRequiredService<ISagaPersistence>();

                var sagas = await persistence.AllSagasToTrigger(
                    options.CurrentValue?.IntervalMs ?? DefaultIntervalMs, stoppingToken);

                foreach (var saga in sagas.ToList())
                {
                    await saga.Trigger(stoppingToken);

                    await persistence.UpdateIfVersionMatches(saga, stoppingToken);
                }

                stopWatch.Stop();

                var remainingDelay =
                    TimeSpan.FromMilliseconds(options.CurrentValue?.IntervalMs ?? DefaultIntervalMs) -
                    stopWatch.Elapsed;

                if (remainingDelay > TimeSpan.Zero)
                {
                    await Task.Delay(remainingDelay, stoppingToken);
                }
            }
        }, stoppingToken);
    }
}