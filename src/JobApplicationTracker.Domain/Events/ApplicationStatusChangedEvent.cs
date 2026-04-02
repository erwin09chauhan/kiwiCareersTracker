namespace JobApplicationTracker.Domain.Events;

public record ApplicationStatusChangedEvent(
    Guid ApplicationId,
    Guid UserId,
    Enums.ApplicationStatus FromStatus,
    Enums.ApplicationStatus ToStatus,
    DateTime ChangedAtUtc);
