using AutoMapper;
using GymDAL.Repo.Abstract.Extra;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class MembershipRepository : Repository<Membership>, IMembershipRepository
    {
        public MembershipRepository(GymDbContext context) : base(context)
        {
        }

    
    }
}