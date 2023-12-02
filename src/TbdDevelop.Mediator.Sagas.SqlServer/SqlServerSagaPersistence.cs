using System.Reflection;
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
    private readonly ILogger<SqlServerSagaPersistence> _logger;

    public SqlServerSagaPersistence(
        IDbContextFactory<SagaDbContext> contextFactory,
        ILogger<SqlServerSagaPersistence> logger
    )
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public TSaga Retrieve<TSaga, TState>(Guid identifier)
        where TSaga : Saga<TState>
        where TState : class, new()
    {
        var saga = Activator.CreateInstance(typeof(TSaga), BindingFlags.Public | BindingFlags.Instance,
            new[] { identifier }) as TSaga ?? throw new InvalidOperationException();

        using var context = _contextFactory.CreateDbContext();

        var sagaFromDb = context.Sagas.SingleOrDefault(s => s.OrchestrationIdentifier == identifier);

        saga.ApplyState(sagaFromDb is not null
            ? JsonSerializer.Deserialize<TState>(sagaFromDb.State) ?? new TState()
            : new TState());

        return saga;
    }

    public async Task Save<TSaga>(TSaga saga, CancellationToken cancellationToken)
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