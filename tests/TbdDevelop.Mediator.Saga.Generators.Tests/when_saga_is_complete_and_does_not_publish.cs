using Mediator;

namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_is_complete_and_does_not_publish
{
    private ISagaPersistence SagaPersistence = null!;
    private IMediator Mediator = null!;
    private Guid OrchestrationIdentifier = new();
    private NonPublishingSagaSampleNotificationHandler Subject = null!;

    public when_saga_is_complete_and_does_not_publish()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void saga_is_removed()
    {
        SagaPersistence
            .Received()
            .Delete(Arg.Is<NonPublishingSaga>(s => s.OrchestrationIdentifier == OrchestrationIdentifier));
    }

    private void Arrange()
    {
        SagaPersistence = Substitute.For<ISagaPersistence>();
        Mediator = Substitute.For<IMediator>();

        Subject = new NonPublishingSagaSampleNotificationHandler(Mediator, SagaPersistence);
    }

    private void Act()
    {
        Subject.Handle(new SampleNotification(), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}