using AutoMapper;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ConstructUsing(src => new UserDto(
                src.Id,
                src.UserName,
                src.Position));
    }
}
