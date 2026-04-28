using AutoMapper;
using JobApplicationTracker.Application.Reminders.Dtos;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Reminders;

public class ReminderMappingProfile : Profile
{
    public ReminderMappingProfile()
    {
        CreateMap<Reminder, ReminderDto>();
    }
}
