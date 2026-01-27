





using GymDAL.Repo.Abstract.Users;
using Microsoft.EntityFrameworkCore;

namespace GymDAL.Repo.Implementation.Users
{
    public class MemberRepository : Repository<Member>, IMemberRepository
    {
        IApplicationUserRepository _applicationUserRepository;
        GymDbContext gymDbContext;

        public MemberRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public override async Task<Member> GetByIdAsync(string id)
        {
            try
            {
                var member = await base._context.Members
               .Include(m => m.FitnessGoal) // Include the FitnessGoal navigation property
                .FirstOrDefaultAsync(m => m.Id == id);
                if (member != null)
                {
                    return member;

                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        public override async Task<IEnumerable<Member>> GetAllAsync()
        {
            try
            {
                var members = await base._context.Members
                    .Include(m => m.FitnessGoal) // Include the FitnessGoal navigation property
                    .ToListAsync();

                return members;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override Task<IEnumerable<Member>> FindAsync(Expression<Func<Member, bool>> predicate)
        {
            try
            {
                var members = base._context.Members
               .Include(m => m.FitnessGoal) // Include the FitnessGoal navigation property
               .Where(predicate);
                return Task.FromResult(members.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw;

            }
        }
        public override async Task<IEnumerable<Member>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                return await _context.Members.Include(m => m.FitnessGoal)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<Member> GetByEmailAsync(string email)
        {
          
            try
            {
                var member =  _context.Members
               .Include(m => m.FitnessGoal) // Include the FitnessGoal navigation property
               .FirstOrDefault(m => m.Email == email);
                if (member != null)
                {
                    return Task.FromResult(member);
                }
                return Task.FromResult<Member>(null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}