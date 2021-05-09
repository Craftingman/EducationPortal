﻿using System;
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
    public class MaterialService : IMaterialService
    {
        private readonly IRepositoryBase<Material> _materialRepository;

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;
        
        public MaterialService(
            IRepositoryBase<Material> materialRepository,
            IConfiguration configuration,
            IMapper mapper)
        {
            _materialRepository = materialRepository;
            _configuration = configuration;
            _mapper = mapper;
        }


        public async Task<ServiceResult<IEnumerable<MaterialViewModel>>> GetMaterialsAsync(string searchStr = "")
        {
            try
            {
                var result = await _materialRepository.FindByConditionAsync(c => c.Name.Contains(searchStr));

                if (result.Success)
                {
                    return ServiceResult<IEnumerable<MaterialViewModel>>.CreateSuccessResult(
                        _mapper.Map<IEnumerable<MaterialViewModel>>(result.Result.ToList()));
                }
                
                return ServiceResult<IEnumerable<MaterialViewModel>>.CreateFailure(result.Exception);
            }
            catch (Exception e)
            {
                return ServiceResult<IEnumerable<MaterialViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddMaterialAsync(MaterialViewModel materialShort)
        {
            if (materialShort == null)
            {
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
                    return ServiceResult.CreateFailure("Incorrect material type.");
                }

                var result = _materialRepository.Create(material);
                if (result.Success)
                {
                    var saveResult = await _materialRepository.SaveAsync();
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