using Integration.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.SqlServer;
using TbdDevelop.Mediator.Sagas.SqlServer.Context;
using Testcontainers.MsSql;
using Xunit;

namespace SqlServer.Integration.Tests.Fixtures;

public class SagaPersistenceFixture : IAsyncLifetime
{
    public Lazy<IServiceProvider> Provider =>
        new(() => _services.BuildServiceProvider());

    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly MsSqlContainer _msSqlContainer;

    public SagaPersistenceFixture()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithPassword("Password1234!")
            .WithDockerEndpoint("npipe://./pipe/docker_engine")
            .WithCleanUp(true) 
            .Build();

        _services.AddPooledDbContextFactory<SagaDbContext>(factory =>
            factory.UseSqlServer(_msSqlContainer.GetConnectionString()));

        _services.AddScoped<ISagaPersistence, SqlServerSagaPersistence>();

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
        await _msSqlContainer.StartAsync();

        var factory = Provider.Value.GetRequiredService<IDbContextFactory<SagaDbContext>>();

        await using var context = await factory.CreateDbContextAsync();

        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }
}