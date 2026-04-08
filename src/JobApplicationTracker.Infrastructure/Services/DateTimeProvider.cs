using JobApplicationTracker.Application.Common.Interfaces;

namespace JobApplicationTracker.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
