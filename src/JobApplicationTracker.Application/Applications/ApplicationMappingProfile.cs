using AutoMapper;
using JobApplicationTracker.Application.Applications.Dtos;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Applications;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<JobApplication, JobApplicationDto>();
    }
}
