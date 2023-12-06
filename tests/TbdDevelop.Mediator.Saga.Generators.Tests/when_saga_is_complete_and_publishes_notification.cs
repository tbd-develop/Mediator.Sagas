namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_is_complete_and_publishes_notification
{
    private ISagaPersistence SagaPersistence;
    private IMediator Mediator;
    private Guid OrchestrationIdentifier = new();
    private PublishingSagaSampleNotificationHandler Subject;

    public when_saga_is_complete_and_publishes_notification()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void publish_is_called()
    {
        Mediator
            .Received()
            .Publish(Arg.Is<CompleteNotification>(n => n.OrchestrationIdentifier == OrchestrationIdentifier));
    }

    [Fact]
    public void saga_is_removed()
    {
        SagaPersistence
            .Received()
            .Delete(Arg.Is<PublishingSaga>(s => s.OrchestrationIdentifier == OrchestrationIdentifier));
    }

    private void Arrange()
    {
        SagaPersistence = Substitute.For<ISagaPersistence>();
        Mediator = Substitute.For<IMediator>();

        Subject = new PublishingSagaSampleNotificationHandler(Mediator, SagaPersistence);
    }

    private void Act()
    {
        Subject.Handle(new SampleNotification(), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}