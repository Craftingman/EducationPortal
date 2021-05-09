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
    public class SkillService : ISkillService
    {
        private readonly IRepositoryBase<Skill> _skillRepository;

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;
        
        public SkillService(
            IRepositoryBase<Skill> skillRepository,
            IConfiguration configuration,
            IMapper mapper)
        {
            _skillRepository = skillRepository;
            _configuration = configuration;
            _mapper = mapper;
        }
        
        public async Task<ServiceResult<IEnumerable<SkillViewModel>>> GetSkillsAsync(string searchStr = "")
        {
            try
            {
                var result = await _skillRepository.FindByConditionAsync(s => s.Name.Contains(searchStr));

                if (result.Success)
                {
                    return ServiceResult<IEnumerable<SkillViewModel>>.CreateSuccessResult(
                        _mapper.Map<IEnumerable<SkillViewModel>>(result.Result.ToList()));
                }
                
                return ServiceResult<IEnumerable<SkillViewModel>>.CreateFailure(result.Exception);
            }
            catch (Exception e)
            {
                return ServiceResult<IEnumerable<SkillViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddSkillAsync(SkillViewModel skillShort)
        {
            if (skillShort == null)
            {
                return ServiceResult.CreateFailure("Skill is null.");
            }
            
            try
            {
                Skill skill = _mapper.Map<Skill>(skillShort);

                var result = _skillRepository.Create(skill);
                if (result.Success)
                {
                    var saveResult = await _skillRepository.SaveAsync();
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
    }
}