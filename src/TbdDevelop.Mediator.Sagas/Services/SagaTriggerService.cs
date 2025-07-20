using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TbdDevelop.Mediator.Sagas.Services;

public class SagaTriggerService(
    IServiceScopeFactory scopeFactory, 
    ILogger<SagaTriggerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // every minute, we should check to see if we need to do anything with saga expiration? '
        // we have to check every minute, because the process to check the sagas, and the process inserting 
        // the sagas is not connected. 
        
        // It would be more efficient if I could leave this process dormant until a saga is inserted, 
        // and verify that it's sooner, or later than any other saga, to set the "awake" time of this process
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            
            
        }
    }
}