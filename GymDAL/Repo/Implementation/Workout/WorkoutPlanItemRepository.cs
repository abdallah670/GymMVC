using AutoMapper;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Implementation
{
    public class WorkoutPlanItemRepository : Repository<WorkoutPlanItem>, IWorkoutPlanItemRepository
    {
        public WorkoutPlanItemRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<WorkoutPlanItem> CreateAsync(WorkoutPlanItem item, string Createdby)
        {
            try
            {
                item.CreatedBy = Createdby;
                item.CreatedAt = DateTime.UtcNow;
                _context.WorkoutPlanItems.Add(item);
                _context.SaveChanges();
                return Task.FromResult(item);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutPlanItem>(null);
            }
        }

        public Task<IEnumerable<WorkoutPlanItem>> GetAsync(Expression<Func<WorkoutPlanItem, bool>>? Filter)
        {
            try
            {
                var query = _context.WorkoutPlanItems.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<WorkoutPlanItem>>(null);
            }
        }

        public Task<WorkoutPlanItem> GetByAsync(Expression<Func<WorkoutPlanItem, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.WorkoutPlanItems.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<WorkoutPlanItem>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutPlanItem>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int itemId, string DeletedBy)
        {
            try
            {
                var item = _context.WorkoutPlanItems.FirstOrDefault(i => i.Id == itemId);
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

        public Task<WorkoutPlanItem> UpdateAsync(WorkoutPlanItem item, string UpdatedBy)
        {
            try
            {
                var existingItem = _context.WorkoutPlanItems.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    _mapper.Map(item, existingItem);
                    existingItem.UpdatedBy = UpdatedBy;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingItem);
                }
                return Task.FromResult<WorkoutPlanItem>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutPlanItem>(null);
            }
        }
    }
}