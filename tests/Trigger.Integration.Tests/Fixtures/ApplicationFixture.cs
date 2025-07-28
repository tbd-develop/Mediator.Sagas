using Integration.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TbdDevelop.Mediator.Sagas.Configuration;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Infrastructure;
using Trigger.Integration.Tests.Sagas;
using Xunit;

namespace Trigger.Integration.Tests.Fixtures;

public class ApplicationFixture : IAsyncLifetime
{
    private IHost? _app;

    public IServiceProvider Provider =>
        _app?.Services ?? throw new InvalidOperationException("Fixture not initialized");

    public CancellationTokenSource CancellationTokenSource { get; } = new();

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public TestOutputRedirect RedirectOutput(ITestOutputHelper output) => new(output);

    public async ValueTask InitializeAsync()
    {
        var appHost = Host.CreateApplicationBuilder();

        appHost.AddSagas(configure =>
        {
            configure.RegisterSaga<SampleTriggerSaga>();

            configure.UseInMemoryPersistence();
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

        await _app.StartAsync(CancellationTokenSource.Token);
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