using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
