namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_is_complete_and_publishes_notification
{
    private readonly ISagaPersistence _persistence = Substitute.For<ISagaPersistence>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ISagaFactory _factory = Substitute.For<ISagaFactory>();
    private Guid OrchestrationIdentifier = Guid.Empty;
    private PublishingSagaSampleNotificationHandler Subject;

    public when_saga_is_complete_and_publishes_notification()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void publish_is_called()
    {
        _mediator
            .Received()
            .Publish(Arg.Is<CompleteNotification>(n => n.OrchestrationIdentifier == OrchestrationIdentifier));
    }

    [Fact]
    public void saga_is_removed()
    {
        _persistence
            .Received()
            .Delete(Arg.Is<PublishingSaga>(s => s.OrchestrationIdentifier == OrchestrationIdentifier));
    }

    private void Arrange()
    {
        Subject = new PublishingSagaSampleNotificationHandler(_mediator, _factory, _persistence);
    }

    private void Act()
    {
        Subject.Handle(new SampleNotification(), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}