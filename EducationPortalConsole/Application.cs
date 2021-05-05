using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using EFCore;
using Microsoft.Extensions.Configuration;

namespace EducationPortalConsole
{
    public class Application
    {
        private IConfiguration _configuration;
        private EPContext _epContext;

        public Application(IConfiguration config, EPContext epContext)
        {
            _configuration = config;
            _epContext = epContext;
        }

        public void StartApp()
        {
            List<int> i = new List<int>();
            var result = i.Where();
        }
        
        //Test methods
        private void InitDbTestData()
        {
            _epContext.Materials.RemoveRange(_epContext.Materials);
            _epContext.Articles.RemoveRange(_epContext.Articles);
            _epContext.Books.RemoveRange(_epContext.Books);
            _epContext.Courses.RemoveRange(_epContext.Courses);
            _epContext.Skills.RemoveRange(_epContext.Skills);
            _epContext.Users.RemoveRange(_epContext.Users);
            _epContext.UserCourses.RemoveRange(_epContext.UserCourses);
            _epContext.UserSkills.RemoveRange(_epContext.UserSkills);
            
            _epContext.SaveChanges();
            
            _epContext.Users.AddRange(new User[]
            {
                new User()
                {
                        Name = "Victor",
                        Surname = "Zherebnyi",
                        Email = "viktor.zherebnyi@nure.ua",
                        Password = "123123123"
                },
                new User()
                {
                    Name = "Ivan",
                    Surname = "Ivanov",
                    Email = "ivan.ivanov@gmail.com",
                    Password = "ivan2007"
                },
                new User()
                {
                    Name = "Fat",
                    Surname = "Cock",
                    Email = "master228@gmail.com",
                    Password = "dungeon1337"
                },
            });
            
            _epContext.Materials.AddRange(new Material[]
            {
                new Article()
                {
                    Name = "Язык C# и платформа .NET Core",
                    MaterialURL = "https://metanit.com/sharp/tutorial/1.1.php",
                    PublishDate = DateTime.Today,
                    Source = "https://metanit.com/"
                },
                new Book()
                {
                    Name = "Выразительный Javascript",
                    MaterialURL = "https://karmazzin.gitbook.io/eloquentjavascript_ru/",
                    Authors = "Марейн Хавербеке",
                    Pages = 482,
                    Format = "PDF",
                    PublishYear = 2019
                },
                new Video()
                {
                    Name = "Учим HTML за 1 час! От Профессионала | HD Remake",
                    MaterialURL = "https://www.youtube.com/watch?v=bWNmJqgri4Q",
                    Resolution = "1920x1080",
                    Duration = new TimeSpan(1, 4, 44)
                }
            });
            
            _epContext.Skills.AddRange(new Skill[]
            {
                new Skill() { Name = ".NET" },
                new Skill() { Name = "JavaScript" },
                new Skill() { Name = "HTML" },
                new Skill() { Name = "CSS" },
            });
            
            _epContext.SaveChanges();
            
            _epContext.Courses.AddRange(new Course[]
            {
                new Course()
                {
                    Name = "Frontend Basics",
                    Description = "Only today you have a perfect opportunity to become a REAL HIPSTER!",
                    
                    Materials = new List<Material>(new Material[]
                    {
                        _epContext.Materials.FirstOrDefault(m => 
                            m.Name == "Выразительный Javascript"),
                        _epContext.Materials.FirstOrDefault(m => 
                            m.Name == "Учим HTML за 1 час! От Профессионала | HD Remake")
                    }),
                    
                    Skills = new List<Skill>(new Skill[]
                    {
                        _epContext.Skills.FirstOrDefault(s => s.Name == "JavaScript"),
                        _epContext.Skills.FirstOrDefault(s => s.Name == "HTML"),
                        _epContext.Skills.FirstOrDefault(s => s.Name == "CSS")
                    })
                },
                new Course()
                {
                    Name = "FullStack .NET Basics",
                    Description = "For real men!",
                    
                    Materials = new List<Material>(new Material[]
                    {
                        _epContext.Materials.FirstOrDefault(m => 
                            m.Name == "Выразительный Javascript"),
                        _epContext.Materials.FirstOrDefault(m => 
                            m.Name == "Учим HTML за 1 час! От Профессионала | HD Remake"),
                        _epContext.Materials.FirstOrDefault(m => 
                            m.Name == "Язык C# и платформа .NET Core"),
                    }),
                    Skills = new List<Skill>(new Skill[]
                    {
                        _epContext.Skills.FirstOrDefault(s => s.Name == "JavaScript"),
                        _epContext.Skills.FirstOrDefault(s => s.Name == "HTML"),
                        _epContext.Skills.FirstOrDefault(s => s.Name == "CSS"),
                        _epContext.Skills.FirstOrDefault(s => s.Name == ".NET")
                    })
                }
            });
            
            _epContext.SaveChanges();
        }
    }
}