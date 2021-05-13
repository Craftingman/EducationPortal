using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Abstractions;
using Core;
using Core.Entities;
using Core.ViewModels;
using DAL.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL
{
    public class CourseService : ICourseService
    {
        private readonly IRepositoryBase<Course> _courseRepository;

        private readonly IRepositoryBase<Material> _materialRepository;

        private readonly IRepositoryBase<Skill> _skillRepository;

        private readonly IRepositoryBase<User> _userRepository;

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;
        
        private readonly ILogger _logger;

        public CourseService(
            IRepositoryBase<Course> courseRepository,
            IRepositoryBase<Material> materialRepository,
            IRepositoryBase<Skill> skillRepository,
            IRepositoryBase<User> userRepository,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<CourseService> logger)
        {
            _courseRepository = courseRepository;
            _materialRepository = materialRepository;
            _skillRepository = skillRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<ServiceResult> CreateAsync(CourseViewModel courseShort, UserViewModel creator)
        {
            if (courseShort == null)
            {
                _logger.LogError("Course is null.");
                
                return ServiceResult.CreateFailure("Course is null.");
            }

            try
            {
                Course course = _mapper.Map<Course>(courseShort);

                if (creator is not null)
                {
                    var userResult = await _userRepository.FindAsync(creator.Id);

                    if (!userResult.Success)
                    {
                        return ServiceResult.CreateFailure("Database error.");
                    }

                    course.Creator = userResult.Result;
                }

                var result = _courseRepository.Create(course);
                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                var saveResult = await _courseRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    _logger.LogError(saveResult.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                courseShort.Id = course.Id;
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> RemoveAsync(int courseId)
        {
            try
            {
                var findResult = await _courseRepository.FindAsync(courseId);
                if (!findResult.Success)
                {
                    _logger.LogError(findResult.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }

                Course course = findResult.Result;

                if (course is null)
                {
                    _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Course with id {courseId} doesn't exist.");
                }

                var result = _courseRepository.Delete(findResult.Result);
                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                var saveResult = await _courseRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    _logger.LogError(saveResult.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCoursesAsync(string searchStr = "")
        {
            try
            {
                var result = await _courseRepository.FindByConditionAsync(c => c.Name.Contains(searchStr));
                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult<IEnumerable<CourseViewModel>>.CreateFailure("Database error.");
                }

                IEnumerable<Course> courses = result.Result;
                if (courses is null)
                {
                    courses = new List<Course>();
                }

                return ServiceResult<IEnumerable<CourseViewModel>>.CreateSuccessResult(
                    _mapper.Map<IEnumerable<CourseViewModel>>(courses.ToList()));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult<IEnumerable<CourseViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<IEnumerable<SkillViewModel>>> GetCourseSkillsAsync(int courseId)
        {
            try
            {
                var result = await _courseRepository.FindAsync(courseId);
                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult<IEnumerable<SkillViewModel>>.CreateFailure("Database error.");
                }
                
                Course course = result.Result;

                if (course is null)
                {
                    _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                    return ServiceResult<IEnumerable<SkillViewModel>>
                        .CreateFailure($"Course with id {courseId} doesn't exist.");
                }

                IEnumerable<Skill> skills = course.Skills.ToList();

                return ServiceResult<IEnumerable<SkillViewModel>>.CreateSuccessResult(
                    _mapper.Map<IEnumerable<SkillViewModel>>(skills));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult<IEnumerable<SkillViewModel>>.CreateFailure(e);
            }
        }
        
        public async Task<ServiceResult<IEnumerable<MaterialViewModel>>> GetCourseMaterialsAsync(int courseId)
        {
            try
            {
                var result = await _courseRepository.FindAsync(courseId);
                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult<IEnumerable<MaterialViewModel>>.CreateFailure("Database error.");
                }
                
                Course course = result.Result;

                if (course is null)
                {
                    _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                    return ServiceResult<IEnumerable<MaterialViewModel>>
                        .CreateFailure($"Course with id {courseId} doesn't exist.");
                }
                
                IEnumerable<Material> materials = course.Materials.ToList();
                
                return ServiceResult<IEnumerable<MaterialViewModel>>.CreateSuccessResult(
                    _mapper.Map<IEnumerable<MaterialViewModel>>(materials));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult<IEnumerable<MaterialViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> UpdateCourseInfoAsync(CourseViewModel courseShort)
        {
            if (courseShort == null)
            {
                _logger.LogError("Course is null");
                
                return ServiceResult.CreateFailure("Course is null.");
            }

            try
            {
                var result = await _courseRepository.FindAsync(courseShort.Id);
                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }

                Course course = result.Result;
                
                if (course is null)
                {
                    _logger.LogError($"Course with id {courseShort.Id} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Course with id {courseShort.Id} doesn't exist.");
                }
                
                course.Name = courseShort.Name;
                course.Description = courseShort.Description;
                    
                var saveResult = await _courseRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    _logger.LogError(saveResult.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddMaterialAsync(int courseId, int materialId)
        {
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseId);
                var materialResult =  await _materialRepository.FindAsync(materialId);

                if (!courseResult.Success || !materialResult.Success)
                {
                    _logger.LogError(
                        String.Concat(materialResult.NonSuccessMessage, courseResult.NonSuccessMessage));
                    
                    return ServiceResult.CreateFailure("Database error.");
                }

                Course course = courseResult.Result;
                Material material = materialResult.Result;

                if (course is null)
                {
                    _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Course with id {courseId} doesn't exist.");
                }
                
                if (material is null)
                {
                    _logger.LogError($"Material with id {materialId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Material with id {materialId} doesn't exist.");
                }
                
                if ((await HasMaterialAsync(courseId, materialId)).Result)
                {
                    _logger.LogError(
                        $"Attempt to double-add material with id {materialId} to course with id {courseId}.");
                    
                    return ServiceResult.CreateFailure(
                        $"Attempt to double-add material with id {materialId} to course with id {courseId}.");
                }

                course.Materials.Add(material);

                var saveResult = await _courseRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    _logger.LogError(saveResult.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> RemoveMaterialAsync(int courseId, int materialId)
        {
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseId);
                var materialResult =  await _materialRepository.FindAsync(materialId);

                if (!courseResult.Success || !materialResult.Success)
                {
                    _logger.LogError(
                        String.Concat(materialResult.NonSuccessMessage, courseResult.NonSuccessMessage));
                    
                    return ServiceResult.CreateFailure("Database error.");
                }

                Course course = courseResult.Result;
                Material material = materialResult.Result;

                if (course is null)
                {
                    _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Course with id {courseId} doesn't exist.");
                }
                
                if (material is null)
                {
                    _logger.LogError($"Material with id {materialId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Material with id {materialId} doesn't exist.");
                }
                
                course.Materials
                    .Remove(course.Materials
                        .FirstOrDefault(m => m.Id == material.Id));

                var saveResult = await _courseRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    _logger.LogError(saveResult.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddSkillAsync(int courseId, int skillId)
        {
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseId);
                var skillResult =  await _skillRepository.FindAsync(skillId);

                if (!courseResult.Success || !skillResult.Success)
                {
                    _logger.LogError(
                        String.Concat(skillResult.NonSuccessMessage, courseResult.NonSuccessMessage));
                    
                    return ServiceResult.CreateFailure("Database error.");
                }

                Course course = courseResult.Result;
                Skill skill = skillResult.Result;

                if (course is null)
                {
                    _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Course with id {courseId} doesn't exist.");
                }
                
                if (skill is null)
                {
                    _logger.LogError($"Skill with id {skillId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Skill with id {skillId} doesn't exist.");
                }
                
                if ((await HasSkillAsync(courseId, skillId)).Result)
                {
                    _logger.LogError(
                        $"Attempt to double-add skill with id {skill} to course with id {courseId}.");
                    
                    return ServiceResult.CreateFailure(
                        $"Attempt to double-add skill with id {skill} to course with id {courseId}.");
                }

                course.Skills.Add(skill);

                var saveResult = await _courseRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    _logger.LogError(saveResult.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> RemoveSkillAsync(int courseId, int skillId)
        {
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseId);
                var skillResult =  await _skillRepository.FindAsync(skillId);

                if (!courseResult.Success || !skillResult.Success)
                {
                    _logger.LogError(
                        String.Concat(skillResult.NonSuccessMessage, courseResult.NonSuccessMessage));
                    
                    return ServiceResult.CreateFailure("Database error.");
                }

                Course course = courseResult.Result;
                Skill skill = skillResult.Result;

                if (course is null)
                {
                    _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Course with id {courseId} doesn't exist.");
                }
                
                if (skill is null)
                {
                    _logger.LogError($"Skill with id {skillId} doesn't exist.");
                    
                    return ServiceResult.CreateFailure($"Skill with id {skillId} doesn't exist.");
                }

                course.Skills
                    .Remove(course.Skills
                        .FirstOrDefault(m => m.Id == skillId));

                var saveResult = await _courseRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    _logger.LogError(saveResult.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<bool>> HasSkillAsync(int courseId, int skillId)
        {
            var result = await _courseRepository.FindAsync(courseId);
            
            if (!result.Success)
            {
                _logger.LogError(result.NonSuccessMessage);
                    
                return ServiceResult<bool>.CreateFailure("Database error.");
            }

            Course course = result.Result;
                
            if (course is null)
            {
                _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                return ServiceResult<bool>.CreateFailure($"Course with id {courseId} doesn't exist.");
            }

            return ServiceResult<bool>.CreateSuccessResult(course.Skills.Select(s => s.Id).Contains(skillId));
        }

        public async Task<ServiceResult<bool>> HasMaterialAsync(int courseId, int materialId)
        {
            var result = await _courseRepository.FindAsync(courseId);
            
            if (!result.Success)
            {
                _logger.LogError(result.NonSuccessMessage);
                    
                return ServiceResult<bool>.CreateFailure("Database error.");
            }

            Course course = result.Result;
                
            if (course is null)
            {
                _logger.LogError($"Course with id {courseId} doesn't exist.");
                    
                return ServiceResult<bool>.CreateFailure($"Course with id {courseId} doesn't exist.");
            }

            return ServiceResult<bool>
                .CreateSuccessResult(course.Materials.Select(m => m.Id).Contains(materialId));
        }
    }
}