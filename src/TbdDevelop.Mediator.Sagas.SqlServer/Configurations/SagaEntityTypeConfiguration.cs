using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using TbdDevelop.Mediator.Sagas.SqlServer.Models;

namespace TbdDevelop.Mediator.Sagas.SqlServer.Configurations;

public class SagaEntityTypeConfiguration(IConfiguration configuration) : IEntityTypeConfiguration<Saga>
{
    public void Configure(EntityTypeBuilder<Saga> builder)
    {
        builder.ToTable(configuration["sagas:tableName"] ?? "Sagas");
        builder.HasKey(k => k.Id);
    }
}