using AutoMapper;
using JobApplicationTracker.Application.Notes.Dtos;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Notes;

public class NoteMappingProfile : Profile
{
    public NoteMappingProfile()
    {
        CreateMap<Note, NoteDto>();
    }
}
