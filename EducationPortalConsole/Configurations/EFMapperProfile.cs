using AutoMapper;
using Core.Entities;
using Core.ViewModels;

namespace EducationPortalConsole.Configurations
{
    public class EFMapperProfile : Profile
    {
        public EFMapperProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>()
                .ForMember("UserName", opt => opt.MapFrom(u => u.Email));
            CreateMap<Course, CourseViewModel>();
            CreateMap<CourseViewModel, Course>();
            CreateMap<Skill, SkillViewModel>();
            CreateMap<SkillViewModel, Skill>();
        }
    }
}