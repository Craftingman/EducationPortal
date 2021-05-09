using System;
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

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;

        public CourseService(
            IRepositoryBase<Course> courseRepository,
            IConfiguration configuration,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
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
                var result = await Task.Run(() => 
                    _courseRepository.FindByCondition(c => c.Name.Contains(searchStr)));

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
                var result = await Task.Run(() => 
                    _courseRepository.Find(courseShort.Id));

                IEnumerable<Skill> skills = result.Result.Skills;

                if (result.Success)
                {
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
                var result = await Task.Run(() => 
                    _courseRepository.Find(courseShort.Id));

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
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> AddMaterialAsync(MaterialViewModel materialShort)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> RemoveMaterialAsync(MaterialViewModel materialShort)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> AddSkillAsync(SkillViewModel materialShort)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> RemoveSkillAsync(SkillViewModel materialShort)
        {
            throw new NotImplementedException();
        }
    }
}