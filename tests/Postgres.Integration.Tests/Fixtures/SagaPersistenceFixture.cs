using Integration.Base;
using Integration.Base.Sagas.Sample;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Infrastructure;
using TbdDevelop.Mediator.Sagas.Postgres;
using TbdDevelop.Mediator.Sagas.Postgres.Context;
using Testcontainers.PostgreSql;
using Xunit;

namespace Postgres.Integration.Tests.Fixtures;

public class SagaPersistenceFixture : IAsyncLifetime
{
    public Lazy<IServiceProvider> Provider =>
        new(() => _services.BuildServiceProvider());

    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly PostgreSqlContainer _sqlContainer;

    public SagaPersistenceFixture()
    {
        _sqlContainer = new PostgreSqlBuilder("postgres:17-alpine")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithPassword("Password1234!")
            .WithDockerEndpoint("npipe://./pipe/docker_engine")
            .WithCleanUp(true)
            .Build();

        _services.AddPooledDbContextFactory<SagaDbContext>(factory =>
            factory.UseNpgsql(_sqlContainer.GetConnectionString()));

        _services.AddScoped<ISagaPersistence, PostgresSagaPersistence>();
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
        await _sqlContainer.StartAsync();

        var factory = Provider.Value.GetRequiredService<IDbContextFactory<SagaDbContext>>();

        await using var context = await factory.CreateDbContextAsync();

        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _sqlContainer.StopAsync();
    }
}