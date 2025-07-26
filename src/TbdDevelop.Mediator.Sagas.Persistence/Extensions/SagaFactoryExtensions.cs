using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.Persistence.Extensions;

public static class SagaFactoryExtensions
{
    public static ISaga BuildSagaFromModel(this ISagaFactory factory, Saga model)
    {
        var type = Type.GetType(model.TypeIdentifier);

        ArgumentNullException.ThrowIfNull(type);

        var saga = factory.Provider.GetRequiredService(type) as ISaga;
        
        saga.NextTriggerTime = model.NextTriggerTime;
        saga.TriggerInterval = model.TriggerInterval;
        saga.LastTriggered = model.LastTriggered;

        saga.ApplyState(model.OrchestrationIdentifier, model.State.FromJson(saga.State.GetType()));

        return saga;
    }

    public static TSaga BuildSagaFromModel<TSaga>(this ISagaFactory factory, Saga model)
        where TSaga : class, ISaga
    {
        var saga = factory.Provider.GetRequiredService<TSaga>();

        saga.NextTriggerTime = model.NextTriggerTime;
        saga.TriggerInterval = model.TriggerInterval;
        saga.LastTriggered = model.LastTriggered;

        saga.ApplyState(model.OrchestrationIdentifier, model.State.FromJson(saga.State.GetType()));

        return saga;
    }
}