using Mediator;
using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;

namespace TbdDevelop.Mediator.Saga.Generators.Tests;

public class when_single_handler_is_implemented_on_saga
{
    
}

public class SampleSaga : Saga<SampleSagaState>,
    IHandle<SampleNotification>
{
    public SampleSaga(Guid orchestrationIdentifier) : base(orchestrationIdentifier)
    {
        IsComplete = false;
    }

    public override bool IsComplete { get; }
    
    public void Handle(SampleNotification @event)
    {
        throw new NotImplementedException();
    }
}

public class SampleSagaState 
{
}

public class SampleNotification : INotification, IOrchestratedNotification
{
    public Guid MyUseableIdentifier { get; set; }
    
    public Func<IOrchestratedNotification, Guid> OrchestrationIdentifier => x => MyUseableIdentifier;
}