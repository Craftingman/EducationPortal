using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Entities;
using Core.ViewModels;

namespace BLL.Abstractions
{
    public interface IMaterialService
    {
        Task<ServiceResult<IEnumerable<MaterialViewModel>>> GetMaterialsAsync(string searchStr = "");
        
        Task<ServiceResult> AddMaterialAsync(MaterialViewModel materialShort);

        Task<ServiceResult<MaterialViewModel>> GetMaterialAsync(int materialId);
    }
}