namespace GymDAL.Repo.Abstract.Users
{
    public interface IMemberRepository : IRepository<Member>
    {
        // GET Operations
       
        Task<IEnumerable<Member>> GetMembersWithWorkoutPlansAsync(int memberId);
        Task<IEnumerable<Member>> GetMembersWithDietPlansAsync(int memberId);
        Task<IEnumerable<Member>> GetMemberWithProgressLogsAsync(int memberId);
        Task<IEnumerable<Member>> GetMemberWithDietAssignmentsAsync(int memberId);
        Task<IEnumerable<Member>> GetMemberWithPaymentsAsync(int memberId);
        Task<IEnumerable<Member>> GetMemberWithMembershipsAsync(int memberId);


      
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
