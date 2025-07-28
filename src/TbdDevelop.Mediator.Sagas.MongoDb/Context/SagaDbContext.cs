using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.MongoDb.Context;

public sealed class SagaDbContext(DbContextOptions<SagaDbContext> options) 
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SagaDbContext).Assembly);
    }
}