using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core;

namespace DAL.Abstractions
{
    public interface IRepositoryBase<T>
    {
        public ServiceResult<T> Find(int id);
        ServiceResult<IQueryable<T>> FindAll();
        ServiceResult<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression);
        ServiceResult Create(T entity);
        ServiceResult Update(T entity);
        ServiceResult Delete(T entity);
    }
}