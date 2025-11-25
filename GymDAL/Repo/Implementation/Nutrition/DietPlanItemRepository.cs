using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class DietPlanItemRepository : Repository<DietPlanItem>, IDietPlanItemRepository
    {
        public DietPlanItemRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<DietPlanItem> CreateAsync(DietPlanItem item, string Createdby)
        {
            try
            {
                item.CreatedBy = Createdby;
                item.CreatedAt = DateTime.UtcNow;
                _context.DietPlanItems.Add(item);
                _context.SaveChanges();
                return Task.FromResult(item);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlanItem>(null);
            }
        }

        public Task<IEnumerable<DietPlanItem>> GetAsync(Expression<Func<DietPlanItem, bool>>? Filter)
        {
            try
            {
                var query = _context.DietPlanItems.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<DietPlanItem>>(null);
            }
        }

        public Task<DietPlanItem> GetByAsync(Expression<Func<DietPlanItem, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.DietPlanItems.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<DietPlanItem>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlanItem>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int itemId, string DeletedBy)
        {
            try
            {
                var item = _context.DietPlanItems.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    item.ToggleStatus(DeletedBy);
                    _context.SaveChanges();
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public Task<DietPlanItem> UpdateAsync(DietPlanItem item, string UpdatedBy)
        {
            try
            {
                var existingItem = _context.DietPlanItems.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    _mapper.Map(item, existingItem);
                    existingItem.UpdatedBy = UpdatedBy;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingItem);
                }
                return Task.FromResult<DietPlanItem>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlanItem>(null);
            }
        }
    }
}