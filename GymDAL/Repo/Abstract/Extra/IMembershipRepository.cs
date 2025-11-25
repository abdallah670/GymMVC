using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Extra
{
    public interface IMembershipRepository : IRepository<Membership>
    {
        // GET Operations
        Task<Membership> GetByAsync(Expression<Func<Membership, bool>>? Filter);
        Task<IEnumerable<Membership>> GetAsync(Expression<Func<Membership, bool>>? Filter);

        // CREATE Operations
        Task<Membership> CreateAsync(Membership membership, string Createdby);

        // UPDATE Operations
        Task<Membership> UpdateAsync(Membership membership, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int membershipId, string DeletedBy);
    }
}