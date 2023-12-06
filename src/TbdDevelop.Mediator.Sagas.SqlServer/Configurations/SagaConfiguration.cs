using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using TbdDevelop.Mediator.Sagas.SqlServer.Models;

namespace TbdDevelop.Mediator.Sagas.SqlServer.Configurations;

public class SagaConfiguration : IEntityTypeConfiguration<Saga>
{
    private readonly IConfiguration _configuration;

    public SagaConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(EntityTypeBuilder<Saga> builder)
    {
        builder.ToTable(_configuration["sagas:tableName"] ?? "Sagas");
        builder.HasKey(k => k.Id);
    }
}