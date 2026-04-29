using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using MapsterMapper;
using GrooveOn.Services.Interfaces;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Model.ResponseObjects;

namespace GrooveOn.Services.Services
{
    public abstract class BaseService<T, TSearch, TEntity> : IService<T, TSearch> where T : class where TSearch : BaseSearchObject where TEntity : class
    {
        private readonly GrooveOnDbContext _context;
        protected readonly IMapper _mapper;

        public BaseService(GrooveOnDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public virtual async Task<PagedResult<T>> GetAsync(TSearch search)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            query = ApplyFilter(query, search);
            query = AddInclude(query, search);

            const int maxPageSize = 100;

            if (search.PageSize.HasValue && search.PageSize.Value > maxPageSize)
            {
                search.PageSize = maxPageSize;
            }

            int? totalCount = null;
            if (search.IncludeTotalCount)
            {
                totalCount = await query.CountAsync();
            }

            if (!search.RetrieveAll)
            {
                if (search.Page.HasValue && search.PageSize.HasValue)
                {
                    query = query.Skip(search.Page.Value * search.PageSize.Value);
                }

                if (search.PageSize.HasValue)
                {
                    query = query.Take(search.PageSize.Value);
                }
            }
            else
            {
                query = query.Take(maxPageSize);
            }

            var list = await query.ToListAsync();

            return new PagedResult<T>
            {
                Items = list.Select(MapToResponse).ToList(),
                TotalCount = totalCount
            };
        }

        protected virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, TSearch search)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> AddInclude(IQueryable<TEntity> query, TSearch search)
        {
            return query;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return null;
            
            return MapToResponse(entity);
        }

        protected virtual T MapToResponse(TEntity entity) {
            return _mapper.Map<T>(entity);
        }
        
    }
} 