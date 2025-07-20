using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.MongoDb.Context;
using TbdDevelop.Mediator.Sagas.Persistence;

namespace TbdDevelop.Mediator.Sagas.MongoDb;

public class MongoDbSagaPersistence(IDbContextFactory<SagaDbContext> contextFactory)
    : SagaPersistenceBase<SagaDbContext>(contextFactory);