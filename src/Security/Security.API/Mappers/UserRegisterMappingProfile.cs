using AutoMapper;
using Common.Grpc;
using Security.API.Models;
using Security.Application.Features.User.RegisterUser;

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

        CreateMap<UserRegisterRequest, RegisterUserRequest>().ConvertUsing(src=>new RegisterUserRequest
        {
            Email = src.Email,
            Password = src.Password,
            Username = src.Nickname,
            PasswordConfirm = src.PasswordConfirm
        });
    }
}