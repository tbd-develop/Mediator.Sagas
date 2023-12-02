using NSubstitute;


namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_single_handler_is_implemented_on_saga_and_saga_exists
{
    public SampleSagaSampleNotificationHandler Subject;
    public ISagaPersistence SagaPersistence;
    public SampleSaga Result;

    public Guid OrchestratingIdentifier = new();
    public string Name = "Name";
    public DateTime DateRegistered = DateTime.UtcNow;

    public when_single_handler_is_implemented_on_saga_and_saga_exists()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void handle_is_called_on_saga_instance()
    {
        Assert.True(Result.HandlerWasCalled);
    }

    [Fact]
    public void notification_data_is_passed_to_saga()
    {
        Assert.Equal(Name, Result.NotificationWas.Name);
        Assert.Equal(DateRegistered, Result.NotificationWas.DateRegistered);
    }

    [Fact]
    public void saga_is_persisted()
    {
        SagaPersistence
            .Received()
            .Save(Arg.Is<SampleSaga>(x => x.OrchestrationIdentifier == OrchestratingIdentifier));
    }

    private void Arrange()
    {
        Result = new SampleSaga(OrchestratingIdentifier);

        SagaPersistence = Substitute.For<ISagaPersistence>();

        SagaPersistence
            .FetchSagaIfExistsByOrchestrationId<SampleSaga>(Arg.Is(OrchestratingIdentifier))
            .Returns(Result);

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