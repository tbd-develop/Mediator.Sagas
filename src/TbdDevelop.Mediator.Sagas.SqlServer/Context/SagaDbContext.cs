using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.SqlServer.Models;

namespace TbdDevelop.Mediator.Sagas.SqlServer.Context;

public sealed class SagaDbContext : DbContext
{
    public DbSet<Saga> Sagas { get; set; } = null!;

    public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options)
    {
    }
}