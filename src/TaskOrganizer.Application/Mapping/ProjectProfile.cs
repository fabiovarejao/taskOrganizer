using AutoMapper;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Application.Mapping;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ConstructUsing(src => new ProjectDto(
                src.Id,
                src.Name,
                src.Description,
                src.UserId,
                src.Tasks.Count));
        CreateMap<CreateProjectDto, Project>();
    }
}
