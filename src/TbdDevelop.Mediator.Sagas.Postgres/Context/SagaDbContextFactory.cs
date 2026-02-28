using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TbdDevelop.Mediator.Sagas.Postgres.Context;

public class SagaDbContextFactory : IDesignTimeDbContextFactory<SagaDbContext>
{
    public SagaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SagaDbContext>();

        // Use a dummy connection string for design-time - migrations don't need a real database
        optionsBuilder.UseNpgsql("Server=localhost;Database=DesignTimeOnly;Trusted_Connection=true;");

        return new SagaDbContext(optionsBuilder.Options);
    }
}