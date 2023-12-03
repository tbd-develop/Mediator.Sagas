// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlServer.Integration.Tests.Infrastructure;
using TbdDevelop.Mediator.Sagas.SqlServer;
using TbdDevelop.Mediator.Sagas.SqlServer.Context;

var builder = new DefaultHostBuilder(args)
    .

builder.Services.AddPooledDbContextFactory<SagaDbContext>(configure =>
    configure.UseSqlServer(builder.Configuration.GetConnectionString("sagas"))
);


var initializer = new Initializer(
    configuration.GetConnectionString("test")!
);

await initializer.Initialize();

var options = new DbContextOptionsBuilder()
    

var persistence = new SqlServerSagaPersistence()