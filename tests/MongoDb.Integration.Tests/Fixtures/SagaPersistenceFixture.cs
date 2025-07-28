using Integration.Base;
using Integration.Base.Sagas.Sample;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Infrastructure;
using TbdDevelop.Mediator.Sagas.MongoDb;
using TbdDevelop.Mediator.Sagas.MongoDb.Context;
using Testcontainers.MongoDb;
using Xunit;

namespace MongoDb.Integration.Tests.Fixtures;

public class SagaPersistenceFixture : IAsyncLifetime
{
    public Lazy<IServiceProvider> Provider =>
        new(() => _services.BuildServiceProvider());

    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly MongoDbContainer _mongoDbContainer;

    public SagaPersistenceFixture()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongodb/mongodb-community-server:latest")
            .WithPassword("Password1234!")
            .WithDockerEndpoint("npipe://./pipe/docker_engine")
            .WithCleanUp(true)
            .Build();

        _services.AddPooledDbContextFactory<SagaDbContext>(factory =>
            factory.UseMongoDB(_mongoDbContainer.GetConnectionString(), "integration-tests"));

        _services.AddScoped<ISagaPersistence, MongoDbSagaPersistence>();
        _services.AddScoped<ISagaFactory, SagaFactory>();
        _services.AddScoped<SampleSaga>();

        _services.AddLogging(builder =>
        {
            builder.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger());
        });
    }

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public TestOutputRedirect RedirectOutput(ITestOutputHelper output) => new(output);

    public async ValueTask InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();

        var factory = Provider.Value.GetRequiredService<IDbContextFactory<SagaDbContext>>();

        await using var context = await factory.CreateDbContextAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _mongoDbContainer.StopAsync();
    }
}