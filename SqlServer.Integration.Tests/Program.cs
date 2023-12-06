// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlServer.Integration.Tests.Data;
using SqlServer.Integration.Tests.Infrastructure;
using TbdDevelop.Mediator.Sagas.SqlServer;
using TbdDevelop.Mediator.Sagas.SqlServer.Context;

var builder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(configure =>
        configure.AddJsonFile("appsettings.json"))
    .ConfigureServices((context, services) =>
    {
        services.AddPooledDbContextFactory<SagaDbContext>(configure =>
            configure.UseSqlServer(context.Configuration.GetConnectionString("sagas"))
        );

        services.AddSingleton<Initializer>((_) =>
            new Initializer(context.Configuration.GetConnectionString("initialize")!));
        services.AddSingleton<SqlServerSagaPersistence>();
    });

var host = builder.Build();

var initializer = host.Services.GetRequiredService<Initializer>();

//await initializer.Initialize();

var persistence = host.Services.GetRequiredService<SqlServerSagaPersistence>();

var saga = new SampleSaga(Guid.NewGuid());

await persistence.Save(saga, CancellationToken.None);