using AutoMapper;
using GymDAL.Repo.Abstract.Extra;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
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