using System;
using System.Linq;
using System.Linq.Expressions;
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

        public ServiceResult<T> Find(int id)
        {
            try
            {
                T result = this.EPContext.Set<T>().Find(id);
                return ServiceResult<T>.CreateSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.CreateFailure(ex);
            }
        }

        public ServiceResult<IQueryable<T>> FindAll()
        {
            try
            {
                IQueryable<T> result = this.EPContext.Set<T>().AsNoTracking();
                return ServiceResult<IQueryable<T>>.CreateSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IQueryable<T>>.CreateFailure(ex);
            }
        }
        public ServiceResult<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression)
        {
            try
            {
                IQueryable<T> result = this.EPContext.Set<T>().Where(expression).AsNoTracking();
                return ServiceResult<IQueryable<T>>.CreateSuccessResult(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IQueryable<T>>.CreateFailure(ex);
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
    }
}