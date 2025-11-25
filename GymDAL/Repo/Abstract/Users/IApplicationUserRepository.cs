namespace GymDAL.Repo.Abstract.Users
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
       
     
      
        // CREATE Operations
        Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password);

    }
}