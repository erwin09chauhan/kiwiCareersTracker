using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.AuditLogs.Dtos;

public class AuditLogEntryDto
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public ApplicationStatus? FromStatus { get; set; }
    public ApplicationStatus ToStatus { get; set; }
    public string? Notes { get; set; }
    public Guid ChangedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
