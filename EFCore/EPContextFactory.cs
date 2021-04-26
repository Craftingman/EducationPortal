using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EFCore
{
    public class EPContextFactory : IDesignTimeDbContextFactory<EPContext>
    {
        public EPContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EPContext>();
            
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath("D:\\University\\GlebsCourse\\EducationalPortal\\EducationPortalConsole");
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();
            
            string connectionString = config.GetConnectionString("EducationPortal");
            optionsBuilder.UseSqlServer(
                connectionString,
                opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));
            return new EPContext(optionsBuilder.Options);
        }
    }
}