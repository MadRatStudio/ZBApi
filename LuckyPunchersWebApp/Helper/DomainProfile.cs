using AutoMapper;
using Infrastructure.Entities;
using Infrastructure.Model.User;

namespace ZBApi.Helper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<AppUser, UserLoginResponseModel>()
                .ForMember(x => x.ImageSrc, opt => opt.ResolveUsing(x => x.Avatar?.Src));
        }
    }
}
