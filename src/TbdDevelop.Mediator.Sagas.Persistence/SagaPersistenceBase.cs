using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Persistence.Extensions;
using TbdDevelop.Mediator.Sagas.Persistence.Models;
using TbdDevelop.Mediator.Sagas.Persistence.Specifications;

namespace TbdDevelop.Mediator.Sagas.Persistence;

public abstract class SagaPersistenceBase<TContext>(
    IDbContextFactory<TContext> contextFactory,
    ISagaFactory sagaFactory)
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
    public async Task<TSaga?> FetchSagaByOrchestrationIdentifier<TSaga>(Guid identifier,
        CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var sagaFromDb = await context.Set<Saga>().SingleOrDefaultAsync(
            s => s.OrchestrationIdentifier == identifier,
            cancellationToken: cancellationToken);

        return sagaFromDb is null ? null : sagaFactory.BuildSagaFromModel<TSaga>(sagaFromDb);
    }

    /// <summary>
    /// Save the current state of the saga 
    /// </summary>
    /// <param name="saga"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TSaga"></typeparam>
    public async Task<bool> UpdateIfVersionMatches<TSaga>(TSaga saga, CancellationToken cancellationToken = default)
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
                Version = 1,
                LastTriggered = null
            });
        }
        else
        {
            if (sagaFromDb.Version != saga.Version)
            {
                return false;
            }

            sagaFromDb.Version += 1;
            sagaFromDb.IsComplete = saga.IsComplete;
            sagaFromDb.State = JsonSerializer.Serialize(saga.State);
        }

        await context.SaveChangesAsync(cancellationToken);

        return true;
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
    /// Will retrieve all current incomplete sagas due to execute 'withinMs' of the current time
    /// </summary>
    /// <param name="withinMs">Millisecond window in which to check triggers</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IEnumerable<ISaga>> AllSagasToTrigger(int withinMs,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var spec = new SagasToTriggerSpec(withinMs);

        var result = from s in spec.Execute(context.Set<Saga>())
                .AsNoTracking()
                .AsEnumerable()
            let saga = sagaFactory.BuildSagaFromModel(s)
            where saga is not null && !saga.IsComplete
            select saga;

        return result.ToList();
    }
}