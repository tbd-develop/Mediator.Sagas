using NSubstitute;

namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_single_handler_is_implemented_but_no_saga_exists
{
    public SampleSagaSampleNotificationHandler Subject;
    public ISagaPersistence SagaPersistence;

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
        SagaPersistence = Substitute.For<ISagaPersistence>();

        SagaPersistence
            .FetchSagaIfExistsByOrchestrationId<SampleSaga>(Arg.Is(OrchestratingIdentifier))
            .Returns(default(SampleSaga));

        Subject = new SampleSagaSampleNotificationHandler(SagaPersistence);
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