using System;
using AutoMapper;
using GitLabServices.GitLabModels;
using TimeTracker.Models;
using TimeTracker.Models.Models;

namespace GitLabServices.Mapper
{
    public static class MapperGitLabData
    {
        public static IMapper Initialize()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GitLabUser, User>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));

                cfg.CreateMap<GitLabProject, Project>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.NumberOfDevelopers, opt => opt.MapFrom(src => src.NumberOfDevelopers))
                    .ForMember(dest => dest.NumberOfIssues, opt => opt.MapFrom(src => src.NumberOfIssues))
                    .ForMember(dest => dest.NumberOfMergeRequests, opt => opt.MapFrom(src => src.NumberOfMergeRequests))
                    .ForMember(dest => dest.TimeSpentOnProject, opt => opt.MapFrom(src => src.TimeSpentOnProject));

                cfg.CreateMap<GitLabDeveloper, Developer>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));

                cfg.CreateMap<GitLabIssue, Issue>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Name))
                    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.TimeSpent, opt => opt.MapFrom(src => src.TimeStats.TotalTimeSpent))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                    .ForMember(dest => dest.MergeRequestsCount, opt => opt.MapFrom(src => src.MergeRequestsCount > 0));

                cfg.CreateMap<GitLabMergeRequest, MergeRequest>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.TotalTimeStats, opt => opt.MapFrom(src => src.TotalTimeStats));

                cfg.CreateMap<GitLabComment, Comment>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Name))
                    .ForMember(dest => dest.IdIOrMR, opt => opt.MapFrom(src => src.IdIOrMR))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                    .ForMember(dest => dest.TotalTimeStats, opt => opt.MapFrom(src => src.TotalTimeStats));

                cfg.CreateMap<GitLabTokenResponse, TokenResponse>()
                    .ForMember(dest => dest.access_token, opt => opt.MapFrom(src => src.access_token))
                    .ForMember(dest => dest.refresh_token, opt => opt.MapFrom(src => src.refresh_token));
            });

            return configuration.CreateMapper();
        }
    }

}

