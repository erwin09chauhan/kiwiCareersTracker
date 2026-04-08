using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Infrastructure.Persistence.Configurations;

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.FromStatus).HasConversion<string>().HasMaxLength(50);
        builder.Property(l => l.ToStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(l => l.Notes).HasMaxLength(1000);

        builder.HasIndex(l => l.ApplicationId);
        builder.HasIndex(l => l.CreatedAtUtc);
    }
}
