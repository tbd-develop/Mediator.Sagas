﻿using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.SqlServer.Context;

public sealed class SagaDbContext(DbContextOptions<SagaDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SagaDbContext).Assembly);

        modelBuilder.HasDefaultSchema("sagas");
    }
}