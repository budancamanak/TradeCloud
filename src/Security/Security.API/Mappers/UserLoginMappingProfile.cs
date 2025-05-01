using AutoMapper;
using Common.Grpc;
using Security.Application.Features.User.LoginUser;

namespace Security.API.Mappers;

public class UserLoginMappingProfile : Profile
{
    public UserLoginMappingProfile()
    {
        // CreateMap<UserRegisterModel, UserRegisterRequest>()
        //     .ConvertUsing(src => new UserRegisterRequest
        //     {
        //         Email = src.Email,
        //         Nickname = src.Username,
        //         Password = src.Password
        //     });
        //
        // CreateMap<UserRegisterRequest, RegisterUserRequest>().ConvertUsing(src=>new RegisterUserRequest
        // {
        //     Email = src.Email,
        //     Password = src.Password,
        //     Username = src.Nickname,
        //     PasswordConfirm = src.PasswordConfirm
        // });
        CreateMap<GrpcUserLoginRequest, LoginUserRequest>()
            .ForMember(f => f.ClientIp, opt => opt.MapFrom((_, _, _, context) => context.Items["ClientIp"]));
    }
}