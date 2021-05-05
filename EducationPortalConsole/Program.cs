using System;
using EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EducationPortalConsole
{
    class Program
    {
        private static IConfiguration _configuration;
        
        static void Main(string[] args)
        {
            Application app = ConfigureServices(new ServiceCollection())
                .BuildServiceProvider()
                .GetRequiredService<Application>();
            
            app.StartApp();
        }

        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddDbContext<EPContext>(
                options => options.UseSqlServer(
                    _configuration.GetConnectionString("EducationPortal")));

            services.AddTransient<Application>();
            services.AddTransient<IConfiguration>(provider => _configuration);

            return services;
        }
    }
}