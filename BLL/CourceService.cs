using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions;
using Core;
using Core.Entities;
using DAL.Abstractions;

namespace BLL
{
    public class CourseService : ICourseService
    {
        private readonly IRepositoryBase<Course> _courseRepository;

        public CourseService(IRepositoryBase<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }
        
        public async Task<ServiceResult> CreateAsync(Course course)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ServiceResult<IEnumerable<Course>>> GetAllAsync()
        {
            var result = _courseRepository.FindAll();

            if (result.Success)
            {
                return ServiceResult<IEnumerable<Course>>.CreateSuccessResult(result.Result.ToList());
            }

            return ServiceResult<IEnumerable<Course>>.CreateFailure(result.Exception);
        }

        public async Task<ServiceResult<IEnumerable<Course>>> FindAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}