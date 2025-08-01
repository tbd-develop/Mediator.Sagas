using TbdDevelop.Mediator.Saga.Generators.Tests.Infrastructure;

namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_single_handler_is_implemented_on_saga_and_saga_exists : ArrangeActBase
{
    private SampleSagaSampleNotificationHandler _subject = null!;
    private readonly ISagaPersistence _sagaPersistence = Substitute.For<ISagaPersistence>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    private SampleSaga _result = null!;
    private readonly Guid _orchestratingIdentifier = new();
    private readonly string _name = "Name";
    private readonly DateTime _dateRegistered = DateTime.UtcNow;

    [Fact]
    public void handle_is_called_on_saga_instance()
    {
        Assert.True(_result.HandlerWasCalled);
    }

    [Fact]
    public void notification_data_is_passed_to_saga()
    {
        Assert.Equal(_name, _result.NotificationWas.Name);
        Assert.Equal(_dateRegistered, _result.NotificationWas.DateRegistered);
    }

    [Fact]
    public void saga_is_persisted()
    {
        _sagaPersistence
            .Received()
            .UpdateIfVersionMatches(Arg.Is<SampleSaga>(x => x.OrchestrationIdentifier == _orchestratingIdentifier),
                Arg.Any<CancellationToken>());
    }

    protected override ValueTask ArrangeAsync()
    {
        _result = new SampleSaga();

        _sagaPersistence
            .FetchSagaByOrchestrationIdentifier<SampleSaga>(Arg.Is(_orchestratingIdentifier))
            .Returns(_result);

        _sagaPersistence.UpdateIfVersionMatches(Arg.Any<SampleSaga>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _subject = new SampleSagaSampleNotificationHandler(_mediator, _sagaPersistence);

        return base.ArrangeAsync();
    }

    protected override async ValueTask ActAsync()
    {
        await _subject.Handle(new SampleNotification()
            {
                MyUseableIdentifier = _orchestratingIdentifier,
                Name = _name,
                DateRegistered = _dateRegistered
            },
            CancellationToken.None);
    }
}