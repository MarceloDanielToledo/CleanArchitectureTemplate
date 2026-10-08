using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations
{
    public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyRecords");
            builder.HasKey(r => r.Key);
            builder.Property(r => r.Key)
                .HasMaxLength(100);

            builder.Property(r => r.RequestHash)
                .HasMaxLength(64)
                .IsRequired();

            builder.Ignore(r => r.IsCompleted);
            builder.HasIndex(r => r.ExpiresOn);
        }
    }
}
