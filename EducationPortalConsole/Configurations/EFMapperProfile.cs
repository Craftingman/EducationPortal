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
            CreateMap<User, UserViewModel>();
            CreateMap<Course, CourseViewModel>();
            CreateMap<CourseViewModel, Course>();
        }
    }
}