using Integration.Base.Sagas.Sample;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.Integration.Tests.Fixtures;
using TbdDevelop.Mediator.Sagas.Contracts;
using Xunit;

namespace MongoDb.Integration.Tests;

public class WhenSagaIsPersisted(
    SagaPersistenceFixture fixture,
    ITestOutputHelper outputHelper) : IClassFixture<SagaPersistenceFixture>
{
    [Fact]
    public async Task state_is_captured()
    {
        var orchestrationId = Guid.NewGuid();
        var sampleId = 909;

        await using (fixture.RedirectOutput(outputHelper))
        {
            // Arrange
            await using var scope = fixture.Provider.Value.CreateAsyncScope();
            
            var factory = scope.ServiceProvider.GetRequiredService<ISagaFactory>();
            
            var saga = factory.CreateSaga<SampleSaga>(orchestrationId);
            
            // Act

            var persistence = scope.ServiceProvider.GetRequiredService<ISagaPersistence>();

            saga.Handle(new SampleNotification(orchestrationId) { Id = sampleId });

            await persistence.Save(saga, CancellationToken.None);

            // Assert

            var retrieved =
                await persistence.FetchSagaByOrchestrationIdentifier<SampleSaga>(orchestrationId,
                    CancellationToken.None);

            Assert.NotNull(retrieved);

            Assert.Equal($"{sampleId}", retrieved.State.Value);
        }
    }

    [Fact]
    public async Task saga_can_be_restored()
    {
        var orchestrationId = Guid.NewGuid();

        await using (fixture.RedirectOutput(outputHelper))
        {
            // Arrange
            await using var scope = fixture.Provider.Value.CreateAsyncScope();
            
            var factory = scope.ServiceProvider.GetRequiredService<ISagaFactory>();
            
            var saga = factory.CreateSaga<SampleSaga>(orchestrationId);

            // Act

            var persistence = scope.ServiceProvider.GetRequiredService<ISagaPersistence>();

            await persistence.Save(saga, CancellationToken.None);

            // Assert

            var retrieved =
                await persistence.FetchSagaByOrchestrationIdentifier<SampleSaga>(orchestrationId,
                    CancellationToken.None);

            Assert.NotNull(retrieved);
        }
    }
}