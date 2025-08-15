namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_is_complete_and_publishes_notification
{
    private readonly ISagaPersistence _persistence = Substitute.For<ISagaPersistence>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ISagaFactory _factory = Substitute.For<ISagaFactory>();
    private PublishingSagaSampleNotificationHandler _subject;

    private readonly Guid _orchestrationIdentifier = Guid.Empty;

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
            .Publish(Arg.Is<CompleteNotification>(n => n.OrchestrationIdentifier == _orchestrationIdentifier));
    }

    [Fact]
    public void saga_is_removed()
    {
        _persistence
            .Received()
            .Delete(Arg.Is<PublishingSaga>(s => s.OrchestrationIdentifier == _orchestrationIdentifier));
    }

    private void Arrange()
    {
        _factory
            .CreateSaga<PublishingSaga>(Arg.Is(_orchestrationIdentifier), Arg.Any<object?>())
            .Returns(_ =>
            {
                var saga = new PublishingSaga();

                saga.ApplyState(_orchestrationIdentifier, 1, null);

                return saga;
            });

        _subject = new PublishingSagaSampleNotificationHandler(_mediator, _factory, _persistence);
    }

    private void Act()
    {
        _subject.Handle(new SampleNotification(), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}