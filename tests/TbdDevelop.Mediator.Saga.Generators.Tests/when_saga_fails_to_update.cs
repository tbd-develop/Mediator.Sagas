using TbdDevelop.Mediator.Sagas.Infrastructure;

namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_fails_to_update
{
    [Fact]
    public async Task saga_update_failed_exception_is_thrown()
    {
        // Arrange
        IMediator mediator = Substitute.For<IMediator>();
        ISagaPersistence persistence = Substitute.For<ISagaPersistence>();
        var orchestrationIdentifier = Guid.NewGuid();

        persistence.UpdateIfVersionMatches(Arg.Any<SampleSaga>(), Arg.Any<CancellationToken>())
            .Returns(false, false, false);

        persistence
            .FetchSagaByOrchestrationIdentifier<SampleSaga>(orchestrationIdentifier, Arg.Any<CancellationToken>())
            .Returns(new SampleSaga());

        var subject = new SampleSagaSampleNotificationHandler(mediator, persistence);

        //Act/Assert 

        await Assert.ThrowsAsync<SagaUpdatedFailedException>(async () =>
        {
            await subject.Handle(new SampleNotification
                {
                    MyUseableIdentifier = orchestrationIdentifier
                },
                CancellationToken.None);
        });
    }
}