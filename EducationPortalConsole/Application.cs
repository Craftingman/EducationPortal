﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.Abstractions;
using Core.Entities;
using EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EducationPortalConsole
{
    public class Application
    {
        private bool exitFlag = false;
        
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private User _currentUser;
        
        //Test Props
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole<int>> _roleManager;
        private EPContext _epContext;
        
        public Application(
            IConfiguration config, 
            EPContext epContext, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole<int>> roleManager,
            IUserService userService)
        {
            _configuration = config;
            _epContext = epContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }
        
        //viktor.zherebnyi@nure.ua 1
        //jesus228@gmail.com 2
        //user@gmail.com 3
        
        public async Task StartApp()
        {
            while (!exitFlag)
            {
                StartMainMenu();
            }
        }

        private void SignIn()
        {
            Console.Clear();
            
            Console.Write("E-Mail: ");
            string email = Console.ReadLine();
            
            Console.Write("Пароль: ");
            string password = Console.ReadLine();

            var result = _userService.SignInUserAsync(email, password).Result;

            if (result.Success)
            {
                _currentUser = result.Result;
                Console.Clear();
                Console.WriteLine("Успешная авторизация.");
                Thread.Sleep(500);
                return;
            }
            
            Console.Clear();
            Console.WriteLine(
                "Неправильный логин или пароль.\n1. В главное меню\n2. Попробовать заново\nВыберите пункт: ");
            int choise = ValidateChoise(Console.ReadLine());

            switch (choise)
            {
                case 1:
                    return;
                case 2:
                    SignIn();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Неправильный ввод.");
                    Thread.Sleep(800);
                    return;
            }
        }

        private void StartMainMenu()
        {
            Console.Clear();
            Console.WriteLine("--- Главное Меню ---");
            Console.WriteLine("0. Выход");
            Console.WriteLine("Выберите пункт: ");
            
            int choise = ValidateChoise(Console.ReadLine());
            switch (choise)
            {
                case 0:
                    this.exitFlag = true;
                    return;
                default:
                    Console.Clear();
                    Console.WriteLine("Неправильный ввод.");
                    Thread.Sleep(800);
                    return;
            }
        }

        private int ValidateChoise(string choise)
        {
            if (int.TryParse(choise, out int correctChoise))
            {
                return correctChoise;
            }

            return -1;
        }

        //Test methods
        private void InitDbTestData()
        {
            _epContext.Materials.RemoveRange(_epContext.Materials);
            _epContext.Articles.RemoveRange(_epContext.Articles);
            _epContext.Books.RemoveRange(_epContext.Books);
            _epContext.Courses.RemoveRange(_epContext.Courses);
            _epContext.Skills.RemoveRange(_epContext.Skills);
            _epContext.UserCourses.RemoveRange(_epContext.UserCourses);
            _epContext.UserSkills.RemoveRange(_epContext.UserSkills);
            
            _epContext.SaveChanges();

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

        private async Task InitIdentity(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            User admin = new User()
            {
                Name = "Viktor",
                Surname = "Zherebnyi",
                Email = "viktor.zherebnyi@nure.ua",
                UserName = "viktor.zherebnyi@nure.ua"
            };
            User moder = new User()
            {
                Name = "Vlad",
                Surname = "Jesus",
                Email = "jesus228@gmail.com",
                UserName = "jesus228@gmail.com"
            };
            User standard = new User()
            {
                Name = "Standard",
                Surname = "User",
                Email = "user@gmail.com",
                UserName = "user@gmail.com"
            };

            await roleManager.CreateAsync(new IdentityRole<int>("admin"));
            await roleManager.CreateAsync(new IdentityRole<int>("moderator"));
            await roleManager.CreateAsync(new IdentityRole<int>("standard"));
            
            IdentityResult result = await userManager.CreateAsync(admin, "1");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
            
            result = await userManager.CreateAsync(moder, "2");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(moder, "moderator");
            }
            
            result = await userManager.CreateAsync(standard, "3");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(standard, "standard");
            }
        }
    }
}