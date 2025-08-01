using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Sagas.Contracts;
using Trigger.Integration.Tests.Fixtures;
using Trigger.Integration.Tests.Sagas;
using Xunit;

namespace Trigger.Integration.Tests;

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

                await persistence.UpdateIfVersionMatches(saga, saga.State.CancellationTokenSource.Token);

                await Task.Delay(TimeSpan.FromSeconds(30), saga.State.CancellationTokenSource.Token);
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