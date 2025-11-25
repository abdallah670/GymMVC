using GymDAL.Entities.External;
using GymDAL.Repo.Abstract.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Repo.Implementation.Extra
{
    public class SubscriptionRepository: Repository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(GymDbContext context) : base(context)
        {
        }

      
        public override Task<Subscription> GetByIdAsync(int id)
        {
            try
            {
                var subscription = _context.Subscriptions
                    .Include(s => s.Membership).Include(s => s.Membership).Include(m => m.DietPlanAssignment).ThenInclude(d => d.DietPlan).
                    Include(s => s.Membership).Include(m => m.WorkoutAssignment).ThenInclude(w => w.WorkoutPlan)
                    .Include(s => s.Member).ThenInclude(s => s.FitnessGoal)
                    .Include(s => s.Payment)
                    .FirstOrDefaultAsync(s => s.Id == id);
                return subscription;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving subscription with ID {id}: {ex.Message}", ex);
            }
        }
        public override Task<Subscription> GetByIdAsync(string id)
        {
            try
            {
                var subscription = _context.Subscriptions
                    .Include(s => s.Membership).Include(s => s.Membership).Include(m => m.DietPlanAssignment).ThenInclude(d => d.DietPlan).
                     Include(s => s.Membership).Include(m => m.WorkoutAssignment).ThenInclude(w => w.WorkoutPlan)
                    .Include(s => s.Member).ThenInclude(s=>s.FitnessGoal)
                    .Include(s => s.Payment)
                    .FirstOrDefaultAsync(s => s.MemberId == id);
                return subscription;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving subscription with Member ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<Subscription> GetMembershipByIdAsync(int Id)
        {
            try
            {
                var subscription =   _context.Subscriptions.Include(m => m.Membership).Include(m => m.DietPlanAssignment).
                    Include(s => s.Membership).Include(m => m.WorkoutAssignment).FirstOrDefault(s => s.Id == Id);
                return subscription;
            }
            catch (Exception ex) {
                throw; 
            }
        }
    }
}
