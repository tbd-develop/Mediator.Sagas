using Microsoft.Extensions.DependencyInjection;
using MongoDb.Integration.Tests.Fixtures;
using MongoDb.Integration.Tests.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;
using Xunit;

namespace MongoDb.Integration.Tests;

public class WhenSagaIsTriggered(
    ApplicationFixture fixture,
    ITestOutputHelper outputHelper) : IClassFixture<ApplicationFixture>
{
    private readonly Guid _sagaOrchestrationId = Guid.NewGuid();

    [Fact]
    public async Task trigger_is_executed_state_is_updated()
    {
        await using (fixture.RedirectOutput(outputHelper))
        {
            // Arrange
            await using var scope = fixture.Provider.CreateAsyncScope();

            var factory = scope.ServiceProvider.GetRequiredService<ISagaFactory>();
            var persistence = scope.ServiceProvider.GetRequiredService<ISagaPersistence>();

            // Act
            try
            {
                var saga = factory.CreateSaga<SampleTriggerSaga>(_sagaOrchestrationId);

                await persistence.UpdateIfVersionMatches(saga, fixture.CancellationTokenSource.Token);

                await Task.Delay(TimeSpan.FromSeconds(30), fixture.CancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
            }

            // Assert 
            var updatedSaga =
                await persistence.FetchSagaByOrchestrationIdentifier<SampleTriggerSaga>(_sagaOrchestrationId,
                    fixture.CancellationTokenSource.Token);

            Assert.NotNull(updatedSaga);
            Assert.True(updatedSaga.State.HasBeenTriggered);
        }
    }
}