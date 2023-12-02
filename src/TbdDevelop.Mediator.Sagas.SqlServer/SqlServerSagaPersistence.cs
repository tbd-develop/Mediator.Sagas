﻿using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.SqlServer.Context;
using TbdDevelop.Mediator.Sagas.SqlServer.Models;

namespace TbdDevelop.Mediator.Sagas.SqlServer;

public class SqlServerSagaPersistence : ISagaPersistence
{
    private readonly IDbContextFactory<SagaDbContext> _contextFactory;
    
    public SqlServerSagaPersistence(
        IDbContextFactory<SagaDbContext> contextFactory
    )
    {
        _contextFactory = contextFactory;
    }

    public async Task<TSaga?> FetchSagaIfExistsByOrchestrationId<TSaga, TState>(Guid identifier, CancellationToken cancellationToken = default)
        where TSaga : Saga<TState>
        where TState : class, new()
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var sagaFromDb = context.Sagas.SingleOrDefault(s => s.OrchestrationIdentifier == identifier);

        if (sagaFromDb is null)
        {
            return null;
        }
        
        var saga = Activator.CreateInstance(typeof(TSaga), identifier) as TSaga ?? throw new InvalidOperationException();

        saga.ApplyState(JsonSerializer.Deserialize<TState>(sagaFromDb.State) ?? new TState());

        return saga;
    }

    public async Task Save<TSaga>(TSaga saga, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var sagaFromDb = context.Sagas.SingleOrDefault(s => s.OrchestrationIdentifier == saga.OrchestrationIdentifier);

        if (sagaFromDb is null)
        {
            context.Sagas.Add(new Saga
            {
                OrchestrationIdentifier = saga.OrchestrationIdentifier,
                State = JsonSerializer.Serialize(saga.State)
            });
        }
        else
        {
            sagaFromDb.State = JsonSerializer.Serialize(saga.State);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}