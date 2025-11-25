using AutoMapper;
using GymDAL.Repo.Abstract.Extra;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<Payment> CreateAsync(Payment payment, string Createdby)
        {
            try
            {
                payment.CreatedBy = Createdby;
                payment.CreatedAt = DateTime.UtcNow;
                _context.Payments.Add(payment);
                _context.SaveChanges();
                return Task.FromResult(payment);
            }
            catch (Exception)
            {
                return Task.FromResult<Payment>(null);
            }
        }

        public Task<IEnumerable<Payment>> GetAsync(Expression<Func<Payment, bool>>? Filter)
        {
            try
            {
                var query = _context.Payments.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<Payment>>(null);
            }
        }

        public Task<Payment> GetByAsync(Expression<Func<Payment, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.Payments.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<Payment>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<Payment>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int paymentId, string DeletedBy)
        {
            try
            {
                var payment = _context.Payments.FirstOrDefault(p => p.Id == paymentId);
                if (payment != null)
                {
                    payment.ToggleStatus(DeletedBy);
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

        public Task<Payment> UpdateAsync(Payment payment, string UpdatedBy)
        {
            try
            {
                var existingPayment = _context.Payments.FirstOrDefault(p => p.Id == payment.Id);
                if (existingPayment != null)
                {
                    _mapper.Map(payment, existingPayment);
                    existingPayment.UpdatedBy = UpdatedBy;
                    existingPayment.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingPayment);
                }
                return Task.FromResult<Payment>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<Payment>(null);
            }
        }
    }
}