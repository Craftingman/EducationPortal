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

namespace BLL
{
    public class CourseService : ICourseService
    {
        private readonly IRepositoryBase<Course> _courseRepository;

        private readonly IRepositoryBase<Material> _materialRepository;

        private readonly IRepositoryBase<Skill> _skillRepository;

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;

        public CourseService(
            IRepositoryBase<Course> courseRepository,
            IRepositoryBase<Material> materialRepository,
            IRepositoryBase<Skill>skillRepository,
            IConfiguration configuration,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _materialRepository = materialRepository;
            _skillRepository = skillRepository;
            _configuration = configuration;
            _mapper = mapper;
        }
        
        public async Task<ServiceResult> CreateAsync(CourseViewModel courseShort, UserViewModel creator)
        {
            if (courseShort == null)
            {
                return ServiceResult.CreateFailure("Course is null.");
            }

            try
            {
                Course course = _mapper.Map<Course>(courseShort);
                
                var result = _courseRepository.Create(course);
                if (result.Success)
                {
                    var saveResult = await _courseRepository.SaveAsync();
                    if (saveResult.Success)
                    {
                        return ServiceResult.CreateSuccessResult();
                    }
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                return ServiceResult.CreateFailure("Database error.");
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> RemoveAsync(CourseViewModel courseShort)
        {
            if (courseShort == null)
            {
                return ServiceResult.CreateFailure("Course is null.");
            }

            try
            {
                Course course = _mapper.Map<Course>(courseShort);

                var result = _courseRepository.Delete(course);

                if (result.Success)
                {
                    var saveResult = await _courseRepository.SaveAsync();
                    if (saveResult.Success)
                    {
                        return ServiceResult.CreateSuccessResult();
                    }
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateFailure("Database error.");
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCoursesAsync(string searchStr = "")
        {
            try
            {
                var result = await _courseRepository.FindByConditionAsync(c => c.Name.Contains(searchStr));

                if (result.Success)
                {
                    return ServiceResult<IEnumerable<CourseViewModel>>.CreateSuccessResult(
                        _mapper.Map<IEnumerable<CourseViewModel>>(result.Result.ToList()));
                }
                
                return ServiceResult<IEnumerable<CourseViewModel>>.CreateFailure(result.Exception);
            }
            catch (Exception e)
            {
                return ServiceResult<IEnumerable<CourseViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<IEnumerable<SkillViewModel>>> GetCourseSkillsAsync(CourseViewModel courseShort)
        {
            try
            {
                var result = await _courseRepository.FindAsync(courseShort.Id);

                if (result.Success)
                {
                    Course course = result.Result;
                    IEnumerable<Skill> skills = course.Skills.ToList();
                    
                    return ServiceResult<IEnumerable<SkillViewModel>>.CreateSuccessResult(
                        _mapper.Map<IEnumerable<SkillViewModel>>(skills));
                }
                
                return ServiceResult<IEnumerable<SkillViewModel>>.CreateFailure(result.Exception);
            }
            catch (Exception e)
            {
                return ServiceResult<IEnumerable<SkillViewModel>>.CreateFailure(e);
            }
        }
        
        public async Task<ServiceResult<IEnumerable<MaterialViewModel>>> GetCourseMaterialsAsync(CourseViewModel courseShort)
        {
            try
            {
                var result = await _courseRepository.FindAsync(courseShort.Id);

                IEnumerable<Material> materials = result.Result.Materials;

                if (result.Success)
                {
                    return ServiceResult<IEnumerable<MaterialViewModel>>.CreateSuccessResult(
                        _mapper.Map<IEnumerable<MaterialViewModel>>(materials));
                }
                
                return ServiceResult<IEnumerable<MaterialViewModel>>.CreateFailure(result.Exception);
            }
            catch (Exception e)
            {
                return ServiceResult<IEnumerable<MaterialViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> UpdateCourseInfoAsync(CourseViewModel courseShort)
        {
            if (courseShort == null)
            {
                return ServiceResult.CreateFailure("Course is null.");
            }

            try
            {
                var result = await _courseRepository.FindAsync(courseShort.Id);
                if (result.Success)
                {
                    result.Result.Name = courseShort.Name;
                    result.Result.Description = courseShort.Description;
                    
                    var saveResult = await _courseRepository.SaveAsync();
                    if (saveResult.Success)
                    {
                        return ServiceResult.CreateSuccessResult();
                    }
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateFailure("Database error.");
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddMaterialAsync(CourseViewModel courseShort, MaterialViewModel materialShort)
        {
            if (materialShort == null)
            {
                return ServiceResult.CreateFailure("Material is null.");
            }
            if (courseShort == null)
            {
                return ServiceResult.CreateFailure("Course is null.");
            }
            
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseShort.Id);
                var materialResult =  await _materialRepository.FindAsync(materialShort.Id);

                if (!courseResult.Success || !materialResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                ICollection<Material> courseMaterials = courseResult.Result.Materials;
                courseMaterials.Add(materialResult.Result);
                
                _courseRepository.Update(courseResult.Result);
                
                var saveResult = await _courseRepository.SaveAsync();
                if (saveResult.Success)
                {
                    return ServiceResult.CreateSuccessResult();
                }
                
                return ServiceResult.CreateFailure("Database error.");
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> RemoveMaterialAsync(CourseViewModel courseShort, MaterialViewModel materialShort)
        {
            if (materialShort == null)
            {
                return ServiceResult.CreateFailure("Material is null.");
            }
            if (courseShort == null)
            {
                return ServiceResult.CreateFailure("Course is null.");
            }
            
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseShort.Id);
                var materialResult =  await _materialRepository.FindAsync(materialShort.Id);

                if (!courseResult.Success || !materialResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                courseResult.Result.Materials
                    .Remove(courseResult.Result.Materials
                        .FirstOrDefault(m => m.Id == materialResult.Result.Id));

                _courseRepository.Update(courseResult.Result);
                
                var saveResult = await _courseRepository.SaveAsync();
                if (saveResult.Success)
                {
                    return ServiceResult.CreateSuccessResult();
                }
                
                return ServiceResult.CreateFailure("Database error.");
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddSkillAsync(CourseViewModel courseShort, SkillViewModel skillShort)
        {
            if (skillShort == null)
            {
                return ServiceResult.CreateFailure("Material is null.");
            }
            if (courseShort == null)
            {
                return ServiceResult.CreateFailure("Course is null.");
            }
            
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseShort.Id);
                var skillResult =  await _skillRepository.FindAsync(skillShort.Id);

                if (!courseResult.Success || !skillResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                ICollection<Skill> courseSkills = courseResult.Result.Skills;
                courseSkills.Add(skillResult.Result);
                
                _courseRepository.Update(courseResult.Result);
                
                var saveResult = await _courseRepository.SaveAsync();
                if (saveResult.Success)
                {
                    return ServiceResult.CreateSuccessResult();
                }
                
                return ServiceResult.CreateFailure("Database error.");
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> RemoveSkillAsync(CourseViewModel courseShort, SkillViewModel skillShort)
        {
            if (skillShort == null)
            {
                return ServiceResult.CreateFailure("Material is null.");
            }
            if (courseShort == null)
            {
                return ServiceResult.CreateFailure("Course is null.");
            }
            
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseShort.Id);
                var skillResult =  await _skillRepository.FindAsync(skillShort.Id);

                if (!courseResult.Success || !skillResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                courseResult.Result.Skills
                    .Remove(courseResult.Result.Skills
                        .FirstOrDefault(m => m.Id == skillResult.Result.Id));

                _courseRepository.Update(courseResult.Result);
                
                var saveResult = await _courseRepository.SaveAsync();
                if (saveResult.Success)
                {
                    return ServiceResult.CreateSuccessResult();
                }
                
                return ServiceResult.CreateFailure("Database error.");
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }
    }
}