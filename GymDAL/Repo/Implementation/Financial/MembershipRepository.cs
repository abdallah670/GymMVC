using AutoMapper;
using GymDAL.Repo.Abstract.Financial;
using System.Linq.Expressions;
using GymDAL.Entities.Financial;

namespace GymDAL.Repo.Implementation.Financial
{
    public class MembershipRepository : Repository<Membership>, IMembershipRepository
    {
        public MembershipRepository(GymDbContext context) : base(context)
        {
        }

    
    }
}