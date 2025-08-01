using TbdDevelop.Mediator.Saga.Generators.Tests.Infrastructure;

namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_saga_version_does_not_match : ArrangeActBase
{
    private SampleSagaSampleNotificationHandler _subject = null!;
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ISagaPersistence _persistence = Substitute.For<ISagaPersistence>();

    private readonly Guid _orchestrationIdentifier = Guid.NewGuid();

    [Fact]
    public void saga_is_retrieved_twice()
    {
        _persistence
            .Received(2)
            .FetchSagaByOrchestrationIdentifier<SampleSaga>(Arg.Is(_orchestrationIdentifier),
                Arg.Any<CancellationToken>());
    }

    protected override ValueTask ArrangeAsync()
    {
        _persistence.UpdateIfVersionMatches(Arg.Any<SampleSaga>(), Arg.Any<CancellationToken>())
            .Returns(false, true);

        _persistence
            .FetchSagaByOrchestrationIdentifier<SampleSaga>(_orchestrationIdentifier, Arg.Any<CancellationToken>())
            .Returns(new SampleSaga());

        _subject = new SampleSagaSampleNotificationHandler(_mediator, _persistence);

        return base.ArrangeAsync();
    }

    protected override async ValueTask ActAsync()
    {
        await _subject.Handle(new SampleNotification 
                { MyUseableIdentifier = _orchestrationIdentifier },
            CancellationToken.None);
    }
}