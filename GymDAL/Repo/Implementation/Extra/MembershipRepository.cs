using AutoMapper;
using GymDAL.Repo.Abstract.Extra;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class MembershipRepository : Repository<Membership>, IMembershipRepository
    {
        public MembershipRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<Membership> CreateAsync(Membership membership, string Createdby)
        {
            try
            {
                membership.CreatedBy = Createdby;
                membership.CreatedAt = DateTime.UtcNow;
                _context.Memberships.Add(membership);
                _context.SaveChanges();
                return Task.FromResult(membership);
            }
            catch (Exception)
            {
                return Task.FromResult<Membership>(null);
            }
        }

        public Task<IEnumerable<Membership>> GetAsync(Expression<Func<Membership, bool>>? Filter)
        {
            try
            {
                var query = _context.Memberships.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<Membership>>(null);
            }
        }

        public Task<Membership> GetByAsync(Expression<Func<Membership, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.Memberships.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<Membership>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<Membership>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int membershipId, string DeletedBy)
        {
            try
            {
                var membership = _context.Memberships.FirstOrDefault(m => m.Id == membershipId);
                if (membership != null)
                {
                    membership.ToggleStatus(DeletedBy);
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

        public Task<Membership> UpdateAsync(Membership membership, string UpdatedBy)
        {
            try
            {
                var existingMembership = _context.Memberships.FirstOrDefault(m => m.Id == membership.Id);
                if (existingMembership != null)
                {
                    _mapper.Map(membership, existingMembership);
                    existingMembership.UpdatedBy = UpdatedBy;
                    existingMembership.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingMembership);
                }
                return Task.FromResult<Membership>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<Membership>(null);
            }
        }
    }
}