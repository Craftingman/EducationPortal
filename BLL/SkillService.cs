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
using Microsoft.Extensions.Logging;

namespace BLL
{
    public class SkillService : ISkillService
    {
        private readonly IRepositoryBase<Skill> _skillRepository;

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;
        
        private readonly ILogger _logger;
        
        public SkillService(
            IRepositoryBase<Skill> skillRepository,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<SkillService> logger)
        {
            _skillRepository = skillRepository;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<ServiceResult<IEnumerable<SkillViewModel>>> GetSkillsAsync(string searchStr = "")
        {
            try
            {
                var result = await _skillRepository.FindByConditionAsync(s => s.Name.Contains(searchStr));

                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult<IEnumerable<SkillViewModel>>.CreateFailure("Database error.");
                }
                
                return ServiceResult<IEnumerable<SkillViewModel>>.CreateSuccessResult(
                    _mapper.Map<IEnumerable<SkillViewModel>>(result.Result.ToList()));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult<IEnumerable<SkillViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddSkillAsync(SkillViewModel skillShort)
        {
            if (skillShort == null)
            {
                _logger.LogError("Skill is null.");
                
                return ServiceResult.CreateFailure("Skill is null.");
            }
            
            try
            {
                Skill skill = _mapper.Map<Skill>(skillShort);

                var result = _skillRepository.Create(skill);
                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                var saveResult = await _skillRepository.SaveAsync();
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
    }
}