using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract.Core;

namespace GymDAL.Repo.Abstract.Users
{
    public interface IWeightLogRepository : IRepository<WeightLog>
    {
     
        Task<IEnumerable<WeightLog>> GetWeightHistoryAsync(string memberId);
    }
}
