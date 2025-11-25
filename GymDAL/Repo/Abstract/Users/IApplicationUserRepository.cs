namespace GymDAL.Repo.Abstract.Users
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        // GET Operations
        Task<ApplicationUser> GetByAsync(Expression<Func<ApplicationUser, bool>>? Filter);
        Task<IEnumerable<ApplicationUser>> GetAsync(Expression<Func<ApplicationUser,bool>>?Filter);
        Task<ApplicationUser> GetUserWithReportsAsync(string userId);
        Task<ApplicationUser> GetUserWithNotificationsAsync(string userId);
        // CREATE Operations
        Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password,string Createdby);
        // UPDATE Operations
        Task<ApplicationUser> UpdateUserAsync(ApplicationUser user,string UpdatedBy);
        // DELETE Operations
        Task<bool> ToggleStatusUserAsync(string userId,string DeletedBy);
     

    }
}