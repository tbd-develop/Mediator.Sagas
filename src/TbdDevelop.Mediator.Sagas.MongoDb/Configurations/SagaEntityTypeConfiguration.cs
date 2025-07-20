using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.EntityFrameworkCore.Extensions;
using TbdDevelop.Mediator.Sagas.Persistence.Models;

namespace TbdDevelop.Mediator.Sagas.MongoDb.Configurations;

public class SagaEntityTypeConfiguration : IEntityTypeConfiguration<Saga>
{
    public void Configure(EntityTypeBuilder<Saga> builder)
    {
        builder.ToCollection("sagas");

        builder.HasKey(k => k.OrchestrationIdentifier);

        builder.Property(k => k.OrchestrationIdentifier)
            .HasElementName("_id");
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public sealed class SagaMap : BsonClassMap<Saga>
    {
        public SagaMap()
        {
            AutoMap();

            MapIdMember(x => x.OrchestrationIdentifier);
        }
    }
}