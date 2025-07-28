using Mediator;

namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_is_complete_and_does_not_publish
{
    private readonly ISagaPersistence _persistence = Substitute.For<ISagaPersistence>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ISagaFactory _factory = Substitute.For<ISagaFactory>();
    private Guid OrchestrationIdentifier = new();
    private NonPublishingSagaSampleNotificationHandler Subject;

    public when_saga_is_complete_and_does_not_publish()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void saga_is_removed()
    {
        _persistence
            .Received()
            .Delete(Arg.Is<NonPublishingSaga>(s => s.OrchestrationIdentifier == OrchestrationIdentifier));
    }

    private void Arrange()
    {
        _factory.CreateSaga<NonPublishingSaga>(Arg.Is<Guid>(id => id == OrchestrationIdentifier), Arg.Any<object?>())
            .Returns(info => new NonPublishingSaga());
        
        Subject = new NonPublishingSagaSampleNotificationHandler(_mediator, _factory, _persistence);
    }

    private void Act()
    {
        Subject.Handle(new SampleNotification(), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}