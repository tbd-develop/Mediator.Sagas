using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.Persistence;
using TbdDevelop.Mediator.Sagas.Postgres.Context;

namespace TbdDevelop.Mediator.Sagas.Postgres;

public class PostgresSagaPersistence(
    IDbContextFactory<SagaDbContext> contextFactory,
    ISagaFactory factory)
    : SagaPersistenceBase<SagaDbContext>(contextFactory, factory);