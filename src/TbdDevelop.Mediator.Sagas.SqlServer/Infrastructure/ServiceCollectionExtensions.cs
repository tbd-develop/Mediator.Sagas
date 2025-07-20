using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TbdDevelop.Mediator.Sagas.Configuration;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.SqlServer.Context;

namespace TbdDevelop.Mediator.Sagas.SqlServer.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static SagaConfiguration UseSqlServer(
        this SagaConfiguration configuration,
        string connectionString)
    {
        configuration.RegisterComponent(collection =>
        {
            collection.AddPooledDbContextFactory<SagaDbContext>(configure =>
                configure.UseSqlServer(connectionString));

            collection.RemoveAll<ISagaPersistence>();

            collection.AddSingleton<ISagaPersistence, SqlServerSagaPersistence>();

            collection.AddHostedService<SqlServerMigrationService>();
        });

        return configuration;
    }

    private sealed class SqlServerMigrationService(IDbContextFactory<SagaDbContext> contextFactory) 
        : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            await context.Database.MigrateAsync(cancellationToken);
        }
        
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}