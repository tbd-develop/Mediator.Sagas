using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Persistence;
using TbdDevelop.Mediator.Sagas.SqlServer.Context;

namespace TbdDevelop.Mediator.Sagas.SqlServer;

public class SqlServerSagaPersistence(
    IDbContextFactory<SagaDbContext> contextFactory,
    ISagaFactory factory)
    : SagaPersistenceBase<SagaDbContext>(contextFactory, factory);