using AutoMapper;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_WebAPI.Models.Auth;

namespace PRN231_ProjectQA_WebAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Auth
            CreateMap<LoginModel, User>();
            CreateMap<LoginGoogleModel, User>();
            CreateMap<ResetPasswordModel, User>();
            CreateMap<SignUpModel, User>();

            //User
            CreateMap<UpdateUserModel, User>();

            CreateMap<User, UserModel>()
              .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            //map User to AddUserModel
            CreateMap<AddUserModel, User>();
            CreateMap<User, AddUserModel>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))

                .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.DOB))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))

                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

        }

    }
}
