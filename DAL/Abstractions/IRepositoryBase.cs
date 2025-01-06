using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core;

namespace DAL.Abstractions
{
    public interface IRepositoryBase<T>
    {
        Task<ServiceResult<T>> FindAsync(int id);
        
        Task<ServiceResult<IEnumerable<T>>> FindAllAsync();
        
        Task<ServiceResult<IEnumerable<T>>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        
        ServiceResult Create(T entity);
        
        ServiceResult Update(T entity);
        
        ServiceResult Delete(T entity);

        Task<ServiceResult> SaveAsync();
    }
}