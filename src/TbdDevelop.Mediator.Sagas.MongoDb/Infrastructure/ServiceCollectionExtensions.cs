using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization;
using TbdDevelop.Mediator.Sagas.Configuration;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.MongoDb.Configurations;
using TbdDevelop.Mediator.Sagas.MongoDb.Context;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.MongoDb.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static SagaConfiguration UseMongoDb(this SagaConfiguration configuration,
        string connectionString,
        string? databaseName = null)
    {
        configuration.RegisterComponent(collection =>
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Saga)))
            {
                BsonClassMap.RegisterClassMap<SagaEntityTypeConfiguration.SagaMap>();
            }

            collection.AddPooledDbContextFactory<SagaDbContext>(configure =>
                configure.UseMongoDB(connectionString, databaseName ?? "Sagas"));

            collection.RemoveAll<ISagaPersistence>();

            collection.AddTransient<ISagaPersistence, MongoDbSagaPersistence>();
        });

        return configuration;
    }
}