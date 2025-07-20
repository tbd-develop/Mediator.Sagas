using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Persistence.Extensions;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.Persistence;

public abstract class SagaPersistenceBase<TContext>(IDbContextFactory<TContext> contextFactory)
    : ISagaPersistence
    where TContext : DbContext
{
    /// <summary>
    /// Using the orchestration id, retrieve the requested Saga. If the Saga does not exist, will return null
    /// </summary>
    /// <param name="identifier">Orchestration Identifier</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TSaga">ISaga type</typeparam>
    /// <returns>Saga or null</returns>
    public async Task<TSaga?> FetchSagaIfExistsByOrchestrationId<TSaga>(Guid identifier,
        CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var sagaFromDb = await context.Set<Saga>().SingleOrDefaultAsync(
            s => s.OrchestrationIdentifier == identifier,
            cancellationToken: cancellationToken);

        if (sagaFromDb is null)
        {
            return null;
        }

        return ConstructSagaFromModel(sagaFromDb) as TSaga;
    }

    /// <summary>
    /// Save the current state of the saga 
    /// </summary>
    /// <param name="saga"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TSaga"></typeparam>
    public async Task Save<TSaga>(TSaga saga, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var sagaFromDb = await context
            .Set<Saga>()
            .SingleOrDefaultAsync(
                s => s.OrchestrationIdentifier == saga.OrchestrationIdentifier,
                cancellationToken: cancellationToken);

        if (sagaFromDb is null)
        {
            context.Set<Saga>().Add(new Saga
            {
                TypeIdentifier = typeof(TSaga).AssemblyQualifiedName!,
                OrchestrationIdentifier = saga.OrchestrationIdentifier,
                State = saga.State.AsJson(),
                NextTriggerTime = saga.NextTriggerTime,
                TriggerInterval = saga.TriggerInterval,
                LastTriggered = null,
                MaximumTriggerCount = saga.MaximumTriggerCount,
            });
        }
        else
        {
            sagaFromDb.IsComplete = saga.IsComplete;
            sagaFromDb.State = JsonSerializer.Serialize(saga.State);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Delete the saga specified, orchestration id is unique
    /// </summary>
    /// <param name="saga"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    public async Task Delete<T>(T saga, CancellationToken cancellationToken = default) where T : class, ISaga
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var sagaFromDb = await context.Set<Saga>().SingleOrDefaultAsync(
            s => s.OrchestrationIdentifier == saga.OrchestrationIdentifier,
            cancellationToken);

        if (sagaFromDb is null)
        {
            return;
        }

        context.Set<Saga>().Remove(sagaFromDb);

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Will retrieve all current incomplete sagas
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IEnumerable<ISaga>> AllSagas(CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var result = from s in context.Set<Saga>()
                .AsNoTracking()
                .AsEnumerable()
            let saga = ConstructSagaFromModel(s)
            where saga is not null && !saga.IsComplete
            select saga;

        return result.AsEnumerable();
    }

    private static ISaga? ConstructSagaFromModel(Saga saga)
    {
        var type = Type.GetType(saga.TypeIdentifier);

        if (type is null)
        {
            return null;
        }

        if (Activator.CreateInstance(type, saga.OrchestrationIdentifier) is not ISaga instance)
        {
            return null;
        }

        instance.NextTriggerTime = saga.NextTriggerTime;
        instance.TriggerInterval = saga.TriggerInterval;
        instance.LastTriggered = saga.LastTriggered;
        instance.MaximumTriggerCount = saga.MaximumTriggerCount;

        instance.ApplyState(saga.State.FromJson(instance.State.GetType()));

        return instance;
    }
}