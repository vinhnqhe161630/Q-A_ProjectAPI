using AutoMapper;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_WebAPI.Models.Auth;
using PRN231_ProjectQA_WebAPI.Models.Comment;
using PRN231_ProjectQA_WebAPI.Models.Post;
using PRN231_ProjectQA_WebAPI.Models.User;

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


                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))

                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<Post, PostModel>()
                    .ForMember(dest => dest.TotalComment, opt => opt.MapFrom(src => src.Comments.Count))
                        .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                            .ForMember(dest => dest.UserImg, opt => opt.MapFrom(src => src.User.Img));


            CreateMap<PostModel, Post>();
            CreateMap<Post, PostDetailsModel>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                 .ForMember(dest => dest.UserImg, opt => opt.MapFrom(src => src.User.Img))
                  .ForMember(dest => dest.TotalComment, opt => opt.MapFrom(src => src.Comments.Count));

            CreateMap<AddPostModel, Post>()
                 .ForMember(dest => dest.Image1, opt => opt.MapFrom(src => src.Img1))
                  .ForMember(dest => dest.Image2, opt => opt.MapFrom(src => src.Img2));

            CreateMap<Comment, CommentModel>()
                   .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.User.Img))
                 .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                  .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<AnswerComment, AnswerModel>()
                 .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.User.Img))
                  .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CommentModel, Comment>();
            CreateMap<AnswerModel, AnswerComment>();

            CreateMap<AddCommentModel, Comment>();
            CreateMap<AddAnswerCommentmodel, AnswerComment>();
        }




    }
}
