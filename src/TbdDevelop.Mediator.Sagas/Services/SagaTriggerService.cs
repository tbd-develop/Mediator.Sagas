using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Sagas.Services;

public class SagaTriggerService(
    IServiceScopeFactory scopeFactory,
    ILogger<SagaTriggerService> logger) : BackgroundService
{
    private const int IntervalMinutes = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // every x minutes, we should check to see if we need to do anything with saga expiration? '
        // we have to check x minute, because the process to check the sagas, and the process inserting 
        // the sagas is not connected. Our other option is to constantly poll the 

        // Scale-out later needs to considered, sagas living across multiple instances of the servers 
        // can't handle 

        // It would be more efficient if I could leave this process dormant until a saga trigger is necessary, 
        // but we would need to shift that awake when a new saga was inserted. 
        while (!stoppingToken.IsCancellationRequested)
        {
            var stopWatch = Stopwatch.StartNew();

            using var scope = scopeFactory.CreateScope();

            var persistence = scope.ServiceProvider.GetRequiredService<ISagaPersistence>();

            var sagas = await persistence.AllSagasToTrigger(IntervalMinutes, stoppingToken);

            foreach (var saga in sagas)
            {
                await saga.Trigger(stoppingToken);

                await persistence.Save(saga, stoppingToken);
            }

            stopWatch.Stop();

            var remainingDelay = TimeSpan.FromMinutes(IntervalMinutes) - stopWatch.Elapsed;

            if (remainingDelay > TimeSpan.Zero)
            {
                await Task.Delay(remainingDelay, stoppingToken);
            }
        }
    }
}