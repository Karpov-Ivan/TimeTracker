using AutoMapper;
using TimeTracker.Models;

namespace TimeTracker.Adapter.Mapper;

public class MappingDatabaseProfile : Profile
{
    public MappingDatabaseProfile()
    {
        CreateMap<User, DataBase.DBModels.User>().ReverseMap();
    }
}