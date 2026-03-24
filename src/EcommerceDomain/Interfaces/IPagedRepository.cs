using EcommerceDomain.Entities;
using EcommerceDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceDomain.Interfaces
{
    public interface IPagedRepository<T> : IRepository<T> where T : class
    {
        public Task<PaginatedList<TResult>> GetPagedAsync<TResult>(
              int pageNumber,
              int pageSize,
              Expression<Func<T, bool>>? filter = null,
              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
              CancellationToken cancellationToken = default);


    
 }

};