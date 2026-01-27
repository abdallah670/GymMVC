using AutoMapper;
using GymDAL.Repo.Abstract.Financial;
using System.Linq.Expressions;
using GymDAL.Entities.Financial;

namespace GymDAL.Repo.Implementation.Financial
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(GymDbContext context) : base(context)
        {
        }

        public PaymentRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

    }
    
}