using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<JobApplication> Applications { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Reminder> Reminders { get; }
    DbSet<Note> Notes { get; }
    DbSet<AuditLogEntry> AuditLogs { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
