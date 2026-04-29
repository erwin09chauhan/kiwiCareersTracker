using AutoMapper;
using JobApplicationTracker.Application.AuditLogs.Dtos;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.AuditLogs;

public class AuditLogMappingProfile : Profile
{
    public AuditLogMappingProfile()
    {
        CreateMap<AuditLogEntry, AuditLogEntryDto>();
    }
}
