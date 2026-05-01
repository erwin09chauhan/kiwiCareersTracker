using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace JobApplicationTracker.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<JobApplication> Applications => Set<JobApplication>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        var dateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }

        modelBuilder.Entity<JobApplication>().HasQueryFilter(a => a.UserId == _currentUserService.UserId);
        modelBuilder.Entity<Contact>().HasQueryFilter(c => c.Application.UserId == _currentUserService.UserId);
        modelBuilder.Entity<Reminder>().HasQueryFilter(r => r.Application.UserId == _currentUserService.UserId);
        modelBuilder.Entity<Note>().HasQueryFilter(n => n.Application.UserId == _currentUserService.UserId);
        modelBuilder.Entity<AuditLogEntry>().HasQueryFilter(l => l.Application.UserId == _currentUserService.UserId);
        modelBuilder.Entity<Notification>().HasQueryFilter(n => n.UserId == _currentUserService.UserId);

        base.OnModelCreating(modelBuilder);
    }
}
