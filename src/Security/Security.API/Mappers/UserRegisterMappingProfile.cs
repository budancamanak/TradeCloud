using AutoMapper;
using Common.Grpc;
using Security.API.Models;

namespace Security.API.Mappers;

public class UserRegisterMappingProfile : Profile
{
    public UserRegisterMappingProfile()
    {
        CreateMap<UserRegisterModel, UserRegisterRequest>()
            .ConvertUsing(src => new UserRegisterRequest
            {
                Email = src.Email,
                Nickname = src.Username,
                Password = src.Password
            });
    }
}