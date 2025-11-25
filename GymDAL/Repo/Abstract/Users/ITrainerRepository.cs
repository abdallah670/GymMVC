namespace GymDAL.Repo.Abstract.Users
{
    public interface ITrainerRepository : IRepository<Trainer>
    {
        Task<Trainer> GetTrainerWithWorkoutPlansAsync(int trainerId);
        Task<Trainer> GetTrainerWithDietPlansAsync(int trainerId);
        Task<Trainer> GetTrainerWithAvailabilitiesAsync(int trainerId);
        Task<Trainer> GetTrainerWithTrainingSessionsAsync(int trainerId);
        Task<Trainer> GetTrainerWithMembershipsAsync(int trainerId);
        
        // GET Operations
        Task<Member> GetByAsync(Expression<Func<Member, bool>>? Filter);
        Task<IEnumerable<Member>> GetAsync(Expression<Func<Member, bool>>? Filter);
        Task<Member> GetWithReportsAsync(string userId);
        Task<Member> GetWithNotificationsAsync(string userId);
        Task<Member> GetWithSystemLogsAsync(string userId);
        // CREATE Operations
        Task<Member> CreateAsync(Member user, string password, string Createdby);
        // UPDATE Operations
        Task<Member> UpdateAsync(Member user, string UpdatedBy);
        // DELETE Operations
        Task<bool> ToggleStatusUserAsync(string userId, string DeletedBy);
    }
}