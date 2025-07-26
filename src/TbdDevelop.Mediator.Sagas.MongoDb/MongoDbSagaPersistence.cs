using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.Contracts;
using TbdDevelop.Mediator.Sagas.MongoDb.Context;
using TbdDevelop.Mediator.Sagas.Persistence;

namespace TbdDevelop.Mediator.Sagas.MongoDb;

public class MongoDbSagaPersistence(
    IDbContextFactory<SagaDbContext> contextFactory,
    ISagaFactory factory)
    : SagaPersistenceBase<SagaDbContext>(contextFactory, factory);