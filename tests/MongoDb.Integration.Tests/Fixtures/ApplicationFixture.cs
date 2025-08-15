using Integration.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDb.Integration.Tests.Sagas;
using Serilog;
using TbdDevelop.Mediator.Sagas.Configuration;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Infrastructure;
using TbdDevelop.Mediator.Sagas.MongoDb.Infrastructure;
using Testcontainers.MongoDb;
using Xunit;

namespace MongoDb.Integration.Tests.Fixtures;

public class ApplicationFixture : IAsyncLifetime
{
    private IHost? _app;
    
    private MongoDbContainer _mongoDbContainer;

    public IServiceProvider Provider =>
        _app?.Services ?? throw new InvalidOperationException("Fixture not initialized");

    public CancellationTokenSource CancellationTokenSource { get; } = new();

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public TestOutputRedirect RedirectOutput(ITestOutputHelper output) => new(output);

    public async ValueTask InitializeAsync()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongodb/mongodb-community-server:latest")
            .WithPassword("Password1234!")
            .WithDockerEndpoint("npipe://./pipe/docker_engine")
            .WithCleanUp(true)
            .Build();
        
        await _mongoDbContainer.StartAsync();

        var appHost = Host.CreateApplicationBuilder();

        appHost.AddSagas(configure =>
        {
            configure.RegisterSaga<SampleTriggerSaga>();

            configure.UseMongoDb(_mongoDbContainer.GetConnectionString(), "integration-tests");
        });

        appHost.Services.AddScoped<ISagaFactory, SagaFactory>();

        appHost.Services.AddLogging(builder =>
        {
            builder.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger());
        });

        _app = appHost.Build();

        await _app!.StartAsync(CancellationTokenSource.Token);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app != null)
        {
            await _app.StopAsync(CancellationTokenSource.Token);

            _app.Dispose();
        }
    }
}