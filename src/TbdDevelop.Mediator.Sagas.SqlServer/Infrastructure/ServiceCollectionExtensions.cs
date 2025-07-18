using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TbdDevelop.Mediator.Sagas.Configuration;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.SqlServer.Context;

namespace TbdDevelop.Mediator.Sagas.SqlServer.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static SagaConfiguration UseSqlServer(this SagaConfiguration configuration, string connectionString)
    {
        configuration.RegisterComponent(collection =>
        {
            collection.AddPooledDbContextFactory<SagaDbContext>(configure =>
                configure.UseSqlServer(connectionString));

            collection.RemoveAll<ISagaPersistence>();
            
            collection.AddSingleton<ISagaPersistence, SqlServerSagaPersistence>();
        });

        return configuration;
    }
}