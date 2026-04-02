namespace JobApplicationTracker.Domain.Entities;

public class JobApplication : Common.BaseEntity
{
    public Guid UserId { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Enums.ApplicationStatus Status { get; set; } = Enums.ApplicationStatus.Applied;
    public DateTime AppliedDate { get; set; }

    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<AuditLogEntry> AuditLogs { get; set; } = new List<AuditLogEntry>();
}
