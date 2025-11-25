using GymDAL.Repo.Abstract.Extra;
using GymDAL.Repo.Abstract.Logs;
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

        public UnitOfWork(GymDbContext context,
                        UserManager<ApplicationUser> userManager,
                       
                        IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
           
            _mapper = mapper;

            // Initialize repositories in correct order to avoid circular dependencies
            ApplicationUsers = new ApplicationUserRepository(_context, _userManager,  _mapper);
            Admins = new AdminRepository(_context,  ApplicationUsers, _mapper);
            Members = new MemberRepository(_context, ApplicationUsers, _mapper);
            Trainers = new TrainerRepository(_context, ApplicationUsers, _mapper);

            // Workout repositories
            WorkoutPlans = new WorkoutPlanRepository(_context, _mapper);
            WorkoutAssignments = new WorkoutAssignmentRepository(_context, _mapper);
            WorkoutLogs = new WorkoutLogRepository(_context, _mapper);
            WorkoutPlanItems = new WorkoutPlanItemRepository(_context, _mapper);

            // Nutrition repositories
            DietPlans = new DietPlanRepository(_context, _mapper);
            DietPlanAssignments = new DietPlanAssignmentRepository(_context, _mapper);
            DietPlanItems = new DietPlanItemRepository(_context, _mapper);
            MealLogs = new MealLogRepository(_context, _mapper);

            // Log repositories
            ProgressLogs = new ProgressLogRepository(_context, _mapper);
            SystemLogs = new SystemLogRepository(_context, _mapper);
            ReportLogs = new ReportLogRepository(_context, _mapper);

            // Extra repositories
            Payments = new PaymentRepository(_context, _mapper);
            Memberships = new MembershipRepository(_context, _mapper);
            TrainerAvailabilities = new TrainerAvailabilityRepository(_context, _mapper);
            TrainingSessions = new TrainingSessionRepository(_context, _mapper);
            Notifications = new NotificationRepository(_context, _mapper);
        }

        // Repository properties
        public IAdminRepository Admins { get; }
        public IApplicationUserRepository ApplicationUsers { get; }
        public IMemberRepository Members { get; }
        public ITrainerRepository Trainers { get; }
        public IWorkoutPlanRepository WorkoutPlans { get; }
        public IDietPlanRepository DietPlans { get; }
        public IWorkoutAssignmentRepository WorkoutAssignments { get; }
        public IDietPlanAssignmentRepository DietPlanAssignments { get; }
        public IWorkoutPlanItemRepository WorkoutPlanItems { get; }
        public IDietPlanItemRepository DietPlanItems { get; }
        public IPaymentRepository Payments { get; }
        public IMembershipRepository Memberships { get; }
        public ITrainerAvailabilityRepository TrainerAvailabilities { get; }
        public ITrainingSessionRepository TrainingSessions { get; }
        public IProgressLogRepository ProgressLogs { get; }
        public INotificationRepository Notifications { get; }
        public IWorkoutLogRepository WorkoutLogs { get; }
        public IMealLogRepository MealLogs { get; }
        public ISystemLogRepository SystemLogs { get; }
        public IReportLogRepository ReportLogs { get; }

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