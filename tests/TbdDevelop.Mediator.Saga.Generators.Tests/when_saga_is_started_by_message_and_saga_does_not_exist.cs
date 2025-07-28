namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_is_started_by_message_and_saga_does_not_exist
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ISagaPersistence _persistence = Substitute.For<ISagaPersistence>();
    private readonly ISagaFactory _factory = Substitute.For<ISagaFactory>();
    private StartedSagaSampleNotificationHandler _subject;

    private readonly Guid _orchestrationIdentifier = Guid.NewGuid();
    private readonly string _name = "Name";
    private readonly DateTime _dateRegistered = DateTime.UtcNow;

    private StartedSaga _result;

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
            .Save(Arg.Is<ISaga>(x => x.OrchestrationIdentifier == _orchestrationIdentifier),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public void saga_state_is_set()
    {
        Assert.NotNull(_result);
        Assert.NotNull(_result.State);

        Assert.Contains(_result.State.Data, f =>
        {
            if (f is SampleNotification notification)
            {
                return notification.OrchestrationIdentifier == _orchestrationIdentifier;
            }

            return false;
        });
    }

    public void Arrange()
    {
        _factory
            .CreateSaga<StartedSaga>(Arg.Is(_orchestrationIdentifier), Arg.Any<object?>())
            .Returns(_ =>
            {
                var saga = new StartedSaga();

                saga.ApplyState(_orchestrationIdentifier, null);

                return saga;
            });

        _persistence
            .When(x => x.Save(Arg.Is<ISaga>(x => x.OrchestrationIdentifier == _orchestrationIdentifier),
                Arg.Any<CancellationToken>())).Do(info => { _result = info.Arg<StartedSaga>(); });

        _subject = new StartedSagaSampleNotificationHandler(_mediator, _factory, _persistence);
    }

    public void Act()
    {
        _subject.Handle(new SampleNotification()
                {
                    Name = _name,
                    DateRegistered = _dateRegistered,
                    MyUseableIdentifier = _orchestrationIdentifier
                },
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}