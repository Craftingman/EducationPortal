using System;
using System.Threading.Tasks;
using AutoMapper;
using BLL;
using BLL.Abstractions;
using Core.Entities;
using DAL;
using DAL.Abstractions;
using EducationPortalConsole.Configurations;
using EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;

namespace EducationPortalConsole
{
    class Program
    {
        private static IConfiguration _configuration;
        
        static async Task Main(string[] args)
        {
            Application app = ConfigureServices(new ServiceCollection())
                .BuildServiceProvider()
                .GetRequiredService<Application>();
            
            await app.StartApp();
        }

        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            
            services.AddLogging(loggerBuilder => loggerBuilder.AddNLog());

            services.AddDbContext<EPContext>(
                options => options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(_configuration.GetConnectionString("EducationPortal")));

            services.AddIdentity<User, IdentityRole<int>>(opts =>
                {
                    opts.Password.RequiredLength = 1;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireDigit = false;
                    opts.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<EPContext>()
                .AddDefaultTokenProviders();
            
            MapperConfiguration mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new EFMapperProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddTransient<IPasswordValidator<User>, EFPasswordValidator>();
            services.AddTransient<IUserValidator<User>, EFUserValidator>();

            services.AddTransient<Application>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IRepositoryBase<Course>, RepositoryBase<Course>>();
            services.AddTransient<IRepositoryBase<Material>, RepositoryBase<Material>>();
            services.AddTransient<IRepositoryBase<Skill>, RepositoryBase<Skill>>();
            services.AddTransient<IRepositoryBase<Course>, RepositoryBase<Course>>();
            services.AddTransient<IConfiguration>(provider => _configuration);

            return services;
        }
    }
}