using AutoMapper;
using TimeTracker.Dto;
using TimeTracker.DTOModels;
using TimeTracker.Models;
using TimeTracker.Models.Models;

namespace TimeTracker.Mapper;

public class MapperDto : Profile
{
    public MapperDto()
    {
        CreateMap<User, UserDto>().ReverseMap();

        CreateMap<Project, ProjectDto>().ReverseMap();

        CreateMap<Issue, IssueDto>().ReverseMap();

        CreateMap<MergeRequest, MergeRequestDto>().ReverseMap();

        CreateMap<Comment, CommentDto>().ReverseMap();

        CreateMap<TokenResponse, TokenResponseDto>().ReverseMap();
    }
}