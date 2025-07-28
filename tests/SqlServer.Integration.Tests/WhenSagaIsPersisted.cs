using Integration.Base.Sagas.Sample;
using Microsoft.Extensions.DependencyInjection;
using SqlServer.Integration.Tests.Fixtures;
using TbdDevelop.Mediator.Sagas.Contracts;
using Xunit;

namespace SqlServer.Integration.Tests;

public class WhenSagaIsPersisted(
    SagaPersistenceFixture fixture,
    ITestOutputHelper outputHelper) : IClassFixture<SagaPersistenceFixture>
{
    private readonly Guid _sagaOrchestrationId = Guid.NewGuid();
    private readonly int _sampleId = 909;

    [Fact]
    public async Task state_is_captured()
    {
        await using (fixture.RedirectOutput(outputHelper))
        {
            // Arrange
            await using var scope = fixture.Provider.Value.CreateAsyncScope();

            var factory = scope.ServiceProvider.GetRequiredService<ISagaFactory>();

            var saga = factory.CreateSaga<SampleSaga>(_sagaOrchestrationId);
            
            // Act

            var persistence = scope.ServiceProvider.GetRequiredService<ISagaPersistence>();

            saga.Handle(new SampleNotification(_sagaOrchestrationId) { Id = _sampleId });

            await persistence.Save(saga, CancellationToken.None);

            // Assert

            var retrieved =
                await persistence.FetchSagaByOrchestrationIdentifier<SampleSaga>(_sagaOrchestrationId,
                    CancellationToken.None);

            Assert.Equal($"{_sampleId}", retrieved.State.Value);
        }
    }

    [Fact]
    public async Task saga_can_be_restored()
    {
        await using (fixture.RedirectOutput(outputHelper))
        {
            // Arrange
            await using var scope = fixture.Provider.Value.CreateAsyncScope();

            var persistence = scope.ServiceProvider.GetRequiredService<ISagaPersistence>();
            var factory = scope.ServiceProvider.GetRequiredService<ISagaFactory>();

            var saga = factory.CreateSaga<SampleSaga>(_sagaOrchestrationId);

            // Act

            await persistence.Save(saga, CancellationToken.None);

            // Assert

            var retrieved =
                await persistence.FetchSagaByOrchestrationIdentifier<SampleSaga>(_sagaOrchestrationId,
                    CancellationToken.None);

            Assert.NotNull(retrieved);
        }
    }
}