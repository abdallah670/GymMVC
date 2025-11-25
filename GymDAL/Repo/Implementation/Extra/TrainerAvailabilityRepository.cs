using AutoMapper;
using GymDAL.Repo.Abstract.Extra;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class TrainerAvailabilityRepository : Repository<TrainerAvailability>, ITrainerAvailabilityRepository
    {
        public TrainerAvailabilityRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<TrainerAvailability> CreateAsync(TrainerAvailability availability, string Createdby)
        {
            try
            {
                availability.CreatedBy = Createdby;
                availability.CreatedAt = DateTime.UtcNow;
                _context.TrainerAvailabilities.Add(availability);
                _context.SaveChanges();
                return Task.FromResult(availability);
            }
            catch (Exception)
            {
                return Task.FromResult<TrainerAvailability>(null);
            }
        }

        public Task<IEnumerable<TrainerAvailability>> GetAsync(Expression<Func<TrainerAvailability, bool>>? Filter)
        {
            try
            {
                var query = _context.TrainerAvailabilities.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<TrainerAvailability>>(null);
            }
        }

        public Task<TrainerAvailability> GetByAsync(Expression<Func<TrainerAvailability, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.TrainerAvailabilities.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<TrainerAvailability>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<TrainerAvailability>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int availabilityId, string DeletedBy)
        {
            try
            {
                var availability = _context.TrainerAvailabilities.FirstOrDefault(a => a.Id == availabilityId);
                if (availability != null)
                {
                    availability.ToggleStatus(DeletedBy);
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

        public Task<TrainerAvailability> UpdateAsync(TrainerAvailability availability, string UpdatedBy)
        {
            try
            {
                var existingAvailability = _context.TrainerAvailabilities.FirstOrDefault(a => a.Id == availability.Id);
                if (existingAvailability != null)
                {
                    _mapper.Map(availability, existingAvailability);
                    existingAvailability.UpdatedBy = UpdatedBy;
                    existingAvailability.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingAvailability);
                }
                return Task.FromResult<TrainerAvailability>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<TrainerAvailability>(null);
            }
        }
    }
}