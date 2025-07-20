using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.SqlServer.Configurations;

public class SagaEntityTypeConfiguration : IEntityTypeConfiguration<Saga>
{
    public void Configure(EntityTypeBuilder<Saga> builder)
    {
        builder.ToTable("Sagas", "sagas");

        builder.HasKey(k => k.OrchestrationIdentifier)
            .HasName("PK_saga_orchestration_id");
    }
}