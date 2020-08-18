using AutoMapper;
using IdentityLearning.Models.ViewModels;
using SharedServices.Models.IdentityModels;

namespace IdentityLearning.Infrastructure.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserListViewModel>();

            CreateMap<UserAccountViewModel, ApplicationUser>()
                .ForMember(c => c.UserName, o => o.MapFrom(y => y.Email));

            CreateMap<ApplicationUser, ChangePasswrodViewModel>()
                .ForMember(c => c.IsExternal, o => o.MapFrom(y => y.IsExternalLogin))
                .ForMember(c => c.UserId, o => o.MapFrom(y => y.Id))
                .ForMember(c => c.Password, o => o.Ignore())
                .ForMember(c => c.OldPassword, o => o.Ignore())
                .ForMember(c => c.ConfirmPassword, o => o.Ignore());

            CreateMap<ApplicationUser, UserUpdateViewModel>()
                .ForMember(c => c.ProfileImageUrl, o => o.Ignore());
        }
    }
}
