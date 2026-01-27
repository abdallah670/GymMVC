using GymDAL.Repo.Implementation.Financial;
using GymDAL.Repo.Implementation.Core;
using GymDAL.Repo.Implementation.Communication;
using GymDAL.Repo.Implementation.Workout;
using GymDAL.Repo.Implementation.Users;
using GymDAL.Repo.Implementation.Nutrition;
using GymDAL.Repo.Abstract.Financial;
using GymDAL.Repo.Abstract.Core;
using GymDAL.Repo.Abstract.Communication;

using GymDAL.Repo.Abstract.Nutrition;
using GymDAL.Repo.Abstract.Users;
using GymDAL.Repo.Abstract.Workout;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace GymDAL.Repo.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
       
        private IDbContextTransaction _transaction;
        private readonly IMapper _mapper;

        public IApplicationUserRepository ApplicationUsers { get; private set; }
        
        public IMemberRepository Members { get; private set; }
        public ITrainerRepository Trainers { get; private set; }
        public ITempRegistrationRepository TempRegistrationRepository { get; private set; }
        public ITrainerReviewRepository TrainerReviews { get; private set; }
        public IWorkoutPlanRepository WorkoutPlans { get; private set; }
        public IDietPlanRepository DietPlans { get; private set; }
        public IWorkoutAssignmentRepository WorkoutAssignments { get; private set; }
        public IDietPlanAssignmentRepository DietPlanAssignments { get; private set; }
        public IWorkoutPlanItemRepository WorkoutPlanItems { get; private set; }
        public IWorkoutLogRepository WorkoutLogs { get; private set; }
        public IDietPlanItemRepository DietPlanItems { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public IMembershipRepository Memberships { get; private set; }
     
        public INotificationRepository Notifications { get; private set; }
        public IChatMessageRepository ChatMessages { get; private set; }
     
        public IMealLogRepository MealLogs { get; private set; }
        // Log-related
        public IWeightLogRepository WeightLogs { get; private set; }

        public ISubscriptionRepository Subscriptions { get; private set; }
    

        public IFitnessGoalsRepository FitnessGoalsRepository  {get; set; }

        public UnitOfWork(GymDbContext context,
                        UserManager<ApplicationUser> userManager,
                       // Add this
                        IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
          
            _mapper = mapper;

            // Initialize repositories in correct order
            InitializeRepositories();
        }

        private void InitializeRepositories()
        {
            // Initialize ApplicationUsers first
            ApplicationUsers = new ApplicationUserRepository(_context, _userManager,  _mapper);

        
            Members = new MemberRepository(_context, _mapper);
            Trainers = new TrainerRepository(_context, _mapper);
            TempRegistrationRepository = new TempRegistrationRepository(_context);
            TrainerReviews = new TrainerReviewRepository(_context);

            // Initialize other repositories (no dependencies)
            WorkoutPlans = new WorkoutPlanRepository(_context, _mapper);
            WorkoutAssignments = new WorkoutAssignmentRepository(_context, _mapper);
        
            WorkoutPlanItems = new WorkoutPlanItemRepository(_context, _mapper);
            WorkoutLogs = new WorkoutLogRepository(_context);

            DietPlans = new DietPlanRepository(_context, _mapper);
            DietPlanAssignments = new DietPlanAssignmentRepository(_context, _mapper);
            DietPlanItems = new DietPlanItemRepository(_context, _mapper);
            MealLogs = new MealLogRepository(_context, _mapper);
            WeightLogs = new WeightLogRepository(_context);

        

            Payments = new PaymentRepository(_context);
            Memberships = new MembershipRepository(_context);
          
            Notifications = new NotificationRepository(_context);
            ChatMessages = new ChatMessageRepository(_context);
            Subscriptions = new SubscriptionRepository(_context);
            FitnessGoalsRepository = new FitnessGoalsRepository(_context);

        }

      

        // Rest of your methods remain the same...
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}