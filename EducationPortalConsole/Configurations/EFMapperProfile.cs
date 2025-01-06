using System;
using AutoMapper;
using Castle.DynamicProxy;
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
            CreateMap<Material, MaterialViewModel>().ForMember("Type", opt =>
            {
                opt.MapFrom(m => MapMaterialType(m));
            });
            CreateMap<MaterialViewModel, Material>();
            CreateMap<Book, BookViewModel>();
            CreateMap<BookViewModel, Book>();
            CreateMap<Article, ArticleViewModel>();
            CreateMap<ArticleViewModel, Article>();
            CreateMap<Video, VideoViewModel>();
            CreateMap<VideoViewModel, Video>();
        }

        private string MapMaterialType(Material m)
        {
            Type t = ProxyUtil.GetUnproxiedType(m);

            if (t == typeof(Book))
            {
                return "Книга";
            }
            if (t == typeof(Article))
            {
                return "Статья";
            }
            if (t == typeof(Video))
            {
                return "Видео";
            }

            return String.Empty;
        }
    }
}