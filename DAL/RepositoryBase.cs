using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core;
using DAL.Abstractions;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private EPContext EPContext { get; set; }
        public RepositoryBase(EPContext repositoryContext)
        {
            this.EPContext = repositoryContext;
        }

        public async Task<ServiceResult<T>> FindAsync(int id)
        {
            try
            {
                T result = await this.EPContext.Set<T>().FindAsync(id);
                EPContext.Entry(result).State = EntityState.Detached;
                
                return ServiceResult<T>.CreateSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.CreateFailure(ex);
            }
        }

        public async Task<ServiceResult<IEnumerable<T>>> FindAllAsync()
        {
            try
            {
                IEnumerable<T> result = await this.EPContext.Set<T>().AsNoTracking().ToListAsync();
                return ServiceResult<IEnumerable<T>>.CreateSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<T>>.CreateFailure(ex);
            }
        }
        public async Task<ServiceResult<IEnumerable<T>>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            try
            {
                IEnumerable<T> result = await this.EPContext.Set<T>().Where(expression).AsNoTracking().ToListAsync();
                return ServiceResult<IEnumerable<T>>.CreateSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<T>>.CreateFailure(ex);
            }
        }
        public ServiceResult Create(T entity)
        {
            try
            {
                this.EPContext.Set<T>().Add(entity);
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return ServiceResult.CreateFailure(ex);
            }
        }
        public ServiceResult Update(T entity)
        {
            try
            {
                this.EPContext.Set<T>().Update(entity);
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return ServiceResult.CreateFailure(ex);
            }
        }
        public ServiceResult Delete(T entity)
        {
            try
            {
                this.EPContext.Set<T>().Remove(entity);
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return ServiceResult.CreateFailure(ex);
            }
        }
        
        public async Task<ServiceResult> SaveAsync() 
        {
            try
            {
                await this.EPContext.SaveChangesAsync();
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }
    }
}