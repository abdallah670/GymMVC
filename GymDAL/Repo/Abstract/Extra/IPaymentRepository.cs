using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Extra
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        // GET Operations
        Task<Payment> GetByAsync(Expression<Func<Payment, bool>>? Filter);
        Task<IEnumerable<Payment>> GetAsync(Expression<Func<Payment, bool>>? Filter);

        // CREATE Operations
        Task<Payment> CreateAsync(Payment payment, string Createdby);

        // UPDATE Operations
        Task<Payment> UpdateAsync(Payment payment, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int paymentId, string DeletedBy);
    }
}