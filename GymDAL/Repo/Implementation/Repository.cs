using GymDAL.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GymDAL.Entities.Core;
using Microsoft.EntityFrameworkCore.Query;

namespace GymDAL.Repo.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly GymDbContext _context;
        private readonly DbSet<T> _dbSet;
        private IMapper mapper;

        public Repository(GymDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public Repository(GymDbContext context, IMapper mapper) : this(context)
        {
            this.mapper = mapper;
        }

        public virtual async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public virtual async Task<T> GetByIdAsync(string id) => await _dbSet.FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.SingleOrDefaultAsync(predicate);

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

        public virtual void Update(T entity) => _dbSet.Update(entity);

        public virtual void Remove(T entity)
        {
            // Soft delete instead of real delete
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsActive = false;
                baseEntity.DeletedAt = DateTime.UtcNow;
                _dbSet.Update(entity);
            }
            else
            {
                // Entity does NOT support soft delete → fallback
                _dbSet.Remove(entity);
            }
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var e in entities)
            {
                Remove(e);
            }
        }

        public virtual async Task<int> CountAsync() => await _dbSet.CountAsync();
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
            return await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

   

    public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
    Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (include != null)
        {
            query = include(query);
        }

        if (orderBy != null)
        {
            return orderBy(query);
        }

        return query;
    }

    // -------------------------------------------------------------
    // NEW SOFT DELETE METHODS
    // -------------------------------------------------------------

    public virtual async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsActive = false;
                _dbSet.Update(entity);
                return true;
            }
            return false;
        }

        public virtual async Task<bool> SoftDeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsActive = false;
                _dbSet.Update(entity);
                return true;
            }
            return false;
        }

        public virtual async Task<bool> RestoreAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsActive = true;
                _dbSet.Update(entity);
                return true;
            }
            return false;
        }

        public virtual async Task<bool> RestoreAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsActive = true;
                _dbSet.Update(entity);
                return true;
            }
            return false;
        }
    }
}
