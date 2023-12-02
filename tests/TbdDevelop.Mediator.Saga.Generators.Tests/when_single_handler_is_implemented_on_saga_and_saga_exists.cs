using Mediator;
using NSubstitute;
using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_single_handler_is_implemented_on_saga_and_saga_exists
{
    public SampleSagaSampleNotificationHandler Subject;
    public ISagaPersistence SagaPersistence;
    public SampleSaga Result;

    public Guid OrchestratingIdentifier = new();

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
        Subject.Handle(new SampleNotification() { MyUseableIdentifier = OrchestratingIdentifier },
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}

public class SampleSaga : Saga<SampleSagaState>,
    IHandle<SampleNotification>
{
    public bool HandlerWasCalled { get; private set; }
    public SampleNotification NotificationWas { get; private set; } = null!;

    public SampleSaga(Guid orchestrationIdentifier) : base(orchestrationIdentifier)
    {
        IsComplete = false;
    }

    public override bool IsComplete { get; }

    public void Handle(SampleNotification @event)
    {
        HandlerWasCalled = true;
        NotificationWas = @event;
    }
}

public class SampleSagaState
{
}

public class SampleNotification : INotification, IOrchestratedNotification
{
    public Guid MyUseableIdentifier { get; set; }

    public Guid OrchestrationIdentifier => MyUseableIdentifier;
}