using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BLL.Abstractions;
using Core.Entities;
using Core.ViewModels;
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
        private readonly ICourseService _courseService;
        private UserViewModel _currentUser;
        
        //Test Props
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole<int>> _roleManager;
        private EPContext _epContext;
        
        public Application(
            IConfiguration config, 
            EPContext epContext, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole<int>> roleManager,
            IUserService userService,
            ICourseService courseService)
        {
            _configuration = config;
            _epContext = epContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _courseService = courseService;
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

        private void StartAccountMenu()
        {
            bool exitAccountMenuFlag = false;

            while (!exitAccountMenuFlag)
            {
                Console.Clear();
                Console.WriteLine("--- Аккаунт ---");
                if (_currentUser is null)
                {
                    Console.WriteLine("1. Вход");
                    Console.WriteLine("2. Регистрация");
                    Console.WriteLine("0. Главное Меню");
                    Console.Write("Выберите пункт: ");
                    
                    switch (ValidateChoise(Console.ReadLine()))
                    {
                        case 0:
                            exitAccountMenuFlag = true;
                            break;
                        case 1:
                            SignIn();
                            break;
                        case 2:
                            RegisterAccount();
                            break;
                        default:
                            DisplayWrongInput();
                            break;
                    }

                    continue;
                }
                
                Console.WriteLine("1. Данные аккаунта");
                Console.WriteLine("2. Мои курсы");
                Console.WriteLine("3. Моя статистика");
                Console.WriteLine("4. Выход из аккаунта");
                Console.WriteLine("0. Главное Меню");
                Console.Write("Выберите пункт: ");
                
                switch (ValidateChoise(Console.ReadLine()))
                {
                    case 0:
                        exitAccountMenuFlag = true;
                        break;
                    case 1:
                        StartAccountDataMenu();
                        break;
                    case 2:
                        StartUserCourseMenu();
                        break;
                    case 3:
                        ShowUserStatistics();
                        break;
                    case 4:
                        exitAccountMenuFlag = true;
                        _currentUser = null;
                        break;
                    default:
                        DisplayWrongInput();
                        break;
                }
            }
        }

        private void StartCoursesMenu()
        {
            bool exitCoursesMenuFlag = false;
            string searchString = "";

            while (!exitCoursesMenuFlag)
            {
                Console.Clear();
                Console.Clear();
                Console.WriteLine("--- Курсы ---");

                List<CourseViewModel> courses = _courseService.GetCoursesAsync(searchString).Result.Result.ToList();
                
                ShowCourses(courses);
                
                Console.WriteLine("1. Поиск");
                Console.WriteLine("2. Выбрать курс");

                if (_currentUser != null)
                {
                    Console.WriteLine("3. Добавить курс");
                }

                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");
                
                switch (ValidateChoise(Console.ReadLine()))
                {
                    case 0:
                        exitCoursesMenuFlag = true;
                        break;
                    case 1:
                        Console.WriteLine($"Поисковая строка сейчас: \'{searchString}\'");
                        Console.Write($"Введите часть названия курса: ");
                        searchString = Console.ReadLine();
                        break;
                    case 2:
                        Console.Write("Введите номер курса: ");
                        int choise = ValidateChoise(Console.ReadLine());
                        
                        if (choise >= 0 && choise < courses.Count)
                        {
                            StartCourseInfoMenu(courses[choise]);
                            break;
                        }
                        
                        Console.WriteLine("Неверный номер курса.");
                        Thread.Sleep(800);
                        break;
                    case 3:
                        if (_currentUser != null)
                        {
                            Console.WriteLine("2. Добавить курс");
                            break;
                        }
                        DisplayWrongInput();
                        break;
                    default:
                        DisplayWrongInput();
                        break;
                }
            }
        }

        private void StartCourseInfoMenu(CourseViewModel course)
        {
            bool exitCourseInfoMenuFlag = false;

            while (!exitCourseInfoMenuFlag)
            {
                Console.Clear();
                Console.WriteLine($"--- Курс ---\n");
                
                if (!ShowCourseInfo(course))
                {
                    StartErrorMenu("Произошла ошибка", out exitCourseInfoMenuFlag);
                }
                
                if (_currentUser != null)
                {
                    Console.WriteLine("1. Добавить в свои курсы");
                }

                bool isAdmin = _userService.HasRoleAsync(_currentUser, "admin").Result.Result;
                bool isModer = _userService.HasRoleAsync(_currentUser, "moderator").Result.Result;

                if (isAdmin || isModer)
                {
                    Console.WriteLine("2. Удалить");
                }

                Console.WriteLine("0. Назад");
                Console.Write("Выберите пункт: ");

                switch (ValidateChoise(Console.ReadLine()))
                {
                    case 0:
                        exitCourseInfoMenuFlag = true;
                        break;
                    default:
                        DisplayWrongInput();
                        break;
                }
            }
        }
        
        private void AddCourse()
        {
            
        }

        private void EditCourseMenu(CourseViewModel course)
        {
            bool editCourseExitFlag = false;

            while (!editCourseExitFlag)
            {
                Console.WriteLine("--- Редактировать курс ---\n");
                Console.WriteLine($"Название: {course.Name}");
                Console.WriteLine($"Описание: {course.Description}");

                var skillsResult = _courseService.GetCourseSkillsAsync(course).Result;
                var materialsResult = _courseService.GetCourseMaterialsAsync(course).Result;

                Console.WriteLine($"Навыки:");
                if (skillsResult.Success)
                {
                    ShowSkills(skillsResult.Result);
                }

                Console.WriteLine("Материалы: ");
                if (materialsResult.Success)
                {
                    ShowMaterials(materialsResult.Result);
                }
                
                Console.WriteLine("1. Добваить навык.");
                Console.WriteLine("2. Удалить навык.");
                Console.WriteLine("3. Добваить материал.");
                Console.WriteLine("4. Удалить материал.");
                Console.WriteLine("5. Изменить описание.");
                Console.WriteLine("6. Изменить название.");
                Console.WriteLine("0. Назад.");
                Console.Write("Выберите пункт: ");

                switch (ValidateChoise(Console.ReadLine()))
                {
                    case 0:
                        editCourseExitFlag = true;
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        Console.WriteLine("Введите новое описание:");
                        string newDesc = Console.ReadLine();
                        
                        break;
                    case 6:
                        Console.WriteLine("Введите новое название:");
                        string newName = Console.ReadLine();
                        
                        break;
                    default:
                        DisplayWrongInput();
                        break;
                }
            }
        }

        private bool ShowCourseInfo(CourseViewModel course)
        {
            Console.WriteLine($"Название: {course.Name}\n");
            Console.WriteLine($"Описание: {course.Description}\n");
            
            var result = _courseService.GetCourseSkillsAsync(course).Result;

            Console.WriteLine($"Навыки: ");
            if (result.Success)
            {
                ShowSkills(result.Result);
                return true;
            }

            return false;
        }
        
        private void ShowMaterials(IEnumerable<MaterialViewModel> materials)
        {
            if (materials == null)
            {
                return;
            }
            Console.WriteLine();
            int i = 0;
            foreach (var material in materials)
            {
                Console.WriteLine($"{i} - {material.Name}");
                i++;
            }
            Console.WriteLine();
        }

        private void ShowSkills(IEnumerable<SkillViewModel> skills)
        {
            if (skills == null)
            {
                return;
            }
            Console.WriteLine();
            int i = 0;
            foreach (var skill in skills)
            {
                Console.WriteLine($"{i} - {skill.Name}");
                i++;
            }
            Console.WriteLine();
        }

        private void ShowCourses(IEnumerable<CourseViewModel> courses)
        {
            if (courses == null)
            {
                return;
            }
            Console.WriteLine();
            int i = 0;
            foreach (var course in courses)
            {
                Console.WriteLine($"{i}. {course.Name}\n");
                i++;
            }
            Console.WriteLine();
        }

        private void StartUsersMenu()
        {
            
        }

        private void StartMainMenu()
        {
            Console.Clear();
            Console.WriteLine("--- Главное Меню ---");
            
            Console.WriteLine("1. Аккаунт");
            Console.WriteLine("2. Курсы");
            
            if (_userService.HasRoleAsync(_currentUser, "admin").Result.Result)
            {
                Console.WriteLine("3. Пользователи");
            }

            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт: ");
            
            switch (ValidateChoise(Console.ReadLine()))
            {
                case 0:
                    this.exitFlag = true;
                    break;
                case 1:
                    StartAccountMenu();
                    break;
                case 2:
                    StartCoursesMenu();
                    break;
                case 3:
                    if (_userService.HasRoleAsync(_currentUser, "admin").Result.Result)
                    {
                        StartUsersMenu();
                        break;
                    }
                    DisplayWrongInput();
                    break;
                default:
                    DisplayWrongInput();
                    return;
            }
        }
        
        private void StartAccountDataMenu()
        {
            
        }
        
        private void StartUserCourseMenu()
        {
            
        }

        private void ShowUserStatistics()
        {
            
        }

        private void RegisterAccount()
        {
            bool registeExitFlag = false;

            while (!registeExitFlag)
            {
                Console.Clear();
            
                Console.WriteLine("Введите данные для регистрации\n");
            
                Console.Write("E-Mail*: ");
                string email = Console.ReadLine();
                
                Console.Write("Имя*: ");
                string name = Console.ReadLine();
                
                Console.Write("Фамилия*: ");
                string surname = Console.ReadLine();
            
                Console.Write("Пароль*: ");
                string password = Console.ReadLine();
                
                Console.Write("Повторите пароль*: ");
                string passwordRep = Console.ReadLine();

                List<string> errorMessages = new List<string>();
                
                if (!Regex.IsMatch(email, _configuration["ValidationPatterns:User:Email"]))
                {
                    errorMessages.Add("Неверный E-Mail.");
                }
                
                if (!Regex.IsMatch(password, _configuration["ValidationPatterns:User:Password"]))
                {
                    errorMessages.Add("Неверный пароль. Длина пароля должна составлять не меньше 8 символов, содержать в себе цифры и буквы латинского алфавита.");
                }
                
                if (!Regex.IsMatch(name, _configuration["ValidationPatterns:User:Name"]))
                {
                    errorMessages.Add("Неверное имя. Имя должно начинаться с большой буквы и состоять из символов русского или латинского алфавита.");
                }
                
                if (!Regex.IsMatch(surname, _configuration["ValidationPatterns:User:Surname"]))
                {
                    errorMessages.Add("Неверная фамилия. Фамилия должна начинаться с большой буквы и состоять из символов русского или латинского алфавита.");
                }
                
                if (password != passwordRep)
                {
                    errorMessages.Add("Пароли не совпадают.");
                }

                if (_userService.UserExistsAsync(email).Result.Result)
                {
                    errorMessages.Add("Пользователь с таким E-Mail уже зарегистрирован.");
                }

                if (errorMessages.Any())
                {
                    Console.Clear();
                    StartErrorMenu(errorMessages, out registeExitFlag);
                    continue;
                }

                UserViewModel user = new UserViewModel()
                {
                    Email = email,
                    Name = name,
                    Surname = surname
                };

                var result = _userService.RegisterUserAsync(user, password).Result;

                if (result.Success)
                {
                    Console.Clear();
                    Console.WriteLine("Успешная регистрация.");
                    Thread.Sleep(800);
                    return;
                }
            
                StartErrorMenu("Произошла ошибка. Попробуйте позже.", out registeExitFlag);
            }
        }

        private void SignIn()
        {
            bool singnInExitFlag = false;

            while (!singnInExitFlag)
            {
                Console.Clear();
            
                Console.WriteLine("Введите данные для входа\n");
            
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
                    Thread.Sleep(800);
                    return;
                }
            
                StartErrorMenu("Неправильный логин или пароль.", out singnInExitFlag);
            }
        }

        private void StartErrorMenu(string errorMessage, out bool outerExitFlag)
        {
            bool errorMenuExitFlag = false;
            outerExitFlag = false;

            while (!errorMenuExitFlag)
            {
                Console.Clear();
                Console.WriteLine($"{errorMessage}\n");
                Console.WriteLine("1. Попробовать заново");
                Console.WriteLine("0. Назад");
                Console.Write("\nВыберите пункт: ");
                
                int choise = ValidateChoise(Console.ReadLine());

                switch (choise)
                {
                    case 0:
                        outerExitFlag = true;
                        errorMenuExitFlag = true; 
                        break;
                    case 1:
                        errorMenuExitFlag = true; 
                        break;
                    default:
                        DisplayWrongInput();
                        break;
                }
            }
        }

        private void StartErrorMenu(IEnumerable<string> errorMessages, out bool outerExitFlag)
        {
            StartErrorMenu(String.Join("\n", errorMessages), out outerExitFlag);
        }

        private void DisplayWrongInput()
        {
            Console.Clear();
            Console.WriteLine("Неправильный ввод.");
            Thread.Sleep(800);
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