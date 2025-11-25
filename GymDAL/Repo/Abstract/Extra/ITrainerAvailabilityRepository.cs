using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Extra
{
    public interface ITrainerAvailabilityRepository : IRepository<TrainerAvailability>
    {
        // GET Operations
        Task<TrainerAvailability> GetByAsync(Expression<Func<TrainerAvailability, bool>>? Filter);
        Task<IEnumerable<TrainerAvailability>> GetAsync(Expression<Func<TrainerAvailability, bool>>? Filter);

        // CREATE Operations
        Task<TrainerAvailability> CreateAsync(TrainerAvailability availability, string Createdby);

        // UPDATE Operations
        Task<TrainerAvailability> UpdateAsync(TrainerAvailability availability, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int availabilityId, string DeletedBy);
    }
}