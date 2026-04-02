namespace JobApplicationTracker.Domain.Entities;

public class AuditLogEntry : Common.BaseEntity
{
    public Guid ApplicationId { get; set; }
    public JobApplication? Application { get; set; }

    public Enums.ApplicationStatus? FromStatus { get; set; }
    public Enums.ApplicationStatus ToStatus { get; set; }
    public string? Notes { get; set; }
    public Guid ChangedByUserId { get; set; }
}
