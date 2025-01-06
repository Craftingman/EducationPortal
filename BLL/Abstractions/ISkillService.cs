using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.ViewModels;

namespace BLL.Abstractions
{
    public interface ISkillService
    {
        Task<ServiceResult<IEnumerable<SkillViewModel>>> GetSkillsAsync(string searchStr = "");
        
        Task<ServiceResult> AddSkillAsync(SkillViewModel skillShort);
    }
}