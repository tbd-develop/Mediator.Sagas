namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_is_started_by_message_and_saga_does_not_exist
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ISagaPersistence _persistence = Substitute.For<ISagaPersistence>();
    private readonly ISagaFactory _factory = Substitute.For<ISagaFactory>();
    public Guid OrchestrationIdentifier = Guid.NewGuid();
    public StartedSagaSampleNotificationHandler Subject;


    public const string Name = "Name";
    public DateTime DateRegistered = DateTime.UtcNow;

    public StartedSaga Result;

    public when_saga_is_started_by_message_and_saga_does_not_exist()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void saga_is_created()
    {
        _persistence
            .Received()
            .Save(Arg.Is<ISaga>(x => x.OrchestrationIdentifier == OrchestrationIdentifier),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public void saga_state_is_set()
    {
        Assert.NotNull(Result);
        Assert.NotNull(Result.State);

        Assert.Contains(Result.State.Data, f =>
        {
            if (f is SampleNotification notification)
            {
                return notification.OrchestrationIdentifier == OrchestrationIdentifier;
            }

            return false;
        });
    }

    public void Arrange()
    {
        _persistence
            .When(x => x.Save(Arg.Is<ISaga>(x => x.OrchestrationIdentifier == OrchestrationIdentifier),
                Arg.Any<CancellationToken>())).Do(info => { Result = info.Arg<StartedSaga>(); });

        Subject = new StartedSagaSampleNotificationHandler(_mediator, _factory, _persistence);
    }

    public void Act()
    {
        Subject.Handle(new SampleNotification()
                {
                    Name = Name,
                    DateRegistered = DateRegistered,
                    MyUseableIdentifier = OrchestrationIdentifier
                },
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}