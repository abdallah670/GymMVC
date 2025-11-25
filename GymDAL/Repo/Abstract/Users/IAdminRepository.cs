namespace GymDAL.Repo.Abstract.Users
{
    public interface IAdminRepository : IRepository<Admin>
    {
        
       
        // GET Operations
        Task<Admin> GetByAsync(Expression<Func<Admin, bool>>? Filter);
        Task<IEnumerable<Admin>> GetAsync(Expression<Func<Admin, bool>>? Filter);
        Task<Admin> GetWithReportsAsync(string userId);
        Task<Admin> GetWithNotificationsAsync(string userId);
        Task<Admin> GetWithSystemLogsAsync(string userId);
        // CREATE Operations
        Task<Admin> CreateAsync(Admin user, string password, string Createdby);
        // UPDATE Operations
        Task<Admin> UpdateAsync(Admin user, string UpdatedBy);
        // DELETE Operations
        Task<bool> ToggleStatusUserAsync(string userId, string currentSuperAdminId);
    }
}