using AutoMapper;
using JobApplicationTracker.Application.Notifications.Dtos;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Notifications;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
