using AutoMapper;
using JobApplicationTracker.Application.Contacts.Dtos;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Contacts;

public class ContactMappingProfile : Profile
{
    public ContactMappingProfile()
    {
        CreateMap<Contact, ContactDto>();
    }
}
