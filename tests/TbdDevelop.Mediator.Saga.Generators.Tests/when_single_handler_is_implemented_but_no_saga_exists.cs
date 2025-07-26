namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_single_handler_is_implemented_but_no_saga_exists
{
    public SampleSagaSampleNotificationHandler Subject;
    public ISagaPersistence SagaPersistence;
    public IMediator Mediator;

    public Guid OrchestratingIdentifier = new();
    public string Name = "Name";
    public DateTime DateRegistered = DateTime.UtcNow;

    public when_single_handler_is_implemented_but_no_saga_exists()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void saga_was_not_persisted()
    {
        SagaPersistence
            .DidNotReceive()
            .Save(Arg.Any<SampleSaga>());
    }

    private void Arrange()
    {
        Mediator = Substitute.For<IMediator>();
        SagaPersistence = Substitute.For<ISagaPersistence>();

        SagaPersistence
            .FetchSagaByOrchestrationIdentifier<SampleSaga>(Arg.Is(OrchestratingIdentifier))
            .Returns(default(SampleSaga));

        Subject = new SampleSagaSampleNotificationHandler(Mediator, SagaPersistence);
    }

    private void Act()
    {
        Subject.Handle(new SampleNotification()
                {
                    MyUseableIdentifier = OrchestratingIdentifier,
                    Name = Name,
                    DateRegistered = DateRegistered
                },
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}