using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Abstractions;
using Castle.DynamicProxy;
using Core;
using Core.Entities;
using Core.ViewModels;
using DAL.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL
{
    public class MaterialService : IMaterialService
    {
        private readonly IRepositoryBase<Material> _materialRepository;

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;

        private readonly ILogger _logger;
        
        public MaterialService(
            IRepositoryBase<Material> materialRepository,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<MaterialService> logger)
        {
            _materialRepository = materialRepository;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<ServiceResult<IEnumerable<MaterialViewModel>>> GetMaterialsAsync(string searchStr = "")
        {
            try
            {
                var result = await _materialRepository.FindByConditionAsync(c => c.Name.Contains(searchStr));

                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult<IEnumerable<MaterialViewModel>>.CreateFailure("Database error.");
                }

                return ServiceResult<IEnumerable<MaterialViewModel>>.CreateSuccessResult(
                    _mapper.Map<IEnumerable<MaterialViewModel>>(result.Result.ToList()));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                
                return ServiceResult<IEnumerable<MaterialViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddMaterialAsync(MaterialViewModel materialShort)
        {
            if (materialShort == null)
            {
                _logger.LogError("Material is null.");
                
                return ServiceResult.CreateFailure("Material is null.");
            }
            
            try
            {
                Type materialType = materialShort.GetType();
                
                Material material;

                if (materialType == typeof(BookViewModel))
                { 
                    material = _mapper.Map<Book>((BookViewModel)materialShort);
                } else if (materialType == typeof(ArticleViewModel))
                { 
                    material = _mapper.Map<Article>((ArticleViewModel)materialShort);
                } else if (materialType == typeof(VideoViewModel))
                { 
                    material = _mapper.Map<Video>((VideoViewModel)materialShort);
                }
                else
                {
                    _logger.LogError("Incorrect material type.");
                    
                    return ServiceResult.CreateFailure("Incorrect material type.");
                }

                var result = _materialRepository.Create(material);
                if (!result.Success)
                {
                    _logger.LogError(result.NonSuccessMessage);
                    
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                var saveResult = await _materialRepository.SaveAsync();
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

        public async Task<ServiceResult<MaterialViewModel>> GetMaterialAsync(int materialId)
        {
            try
            {
                var materialResult = await _materialRepository.FindAsync(materialId);
                
                if (!materialResult.Success)
                {
                    _logger.LogError(materialResult.NonSuccessMessage);
                    
                    return ServiceResult<MaterialViewModel>.CreateFailure("Database error.");
                }

                Material material = materialResult.Result;
                
                if (material is null)
                {
                    string message = $"Material with id {materialId} doesn't exist.";
                    
                    _logger.LogError(message);
                    
                    return ServiceResult<MaterialViewModel>.CreateFailure(message, 404);
                }

                Type materialType = ProxyUtil.GetUnproxiedType(material);

                if (materialType == typeof(Book))
                {
                    return ServiceResult<MaterialViewModel>.CreateSuccessResult(
                        _mapper.Map<BookViewModel>((Book)material));
                } 
                if (materialType == typeof(Article))
                {
                    return ServiceResult<MaterialViewModel>.CreateSuccessResult(
                        _mapper.Map<ArticleViewModel>((Article)material));
                }
                if (materialType == typeof(Video))
                {
                    return ServiceResult<MaterialViewModel>.CreateSuccessResult(
                        _mapper.Map<VideoViewModel>((Video)material));
                }
                
                _logger.LogError("Incorrect material type.");
                
                return ServiceResult<MaterialViewModel>.CreateFailure("Incorrect material type.");
            } 
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return ServiceResult<MaterialViewModel>.CreateFailure(e);
            }
        }
    }
}