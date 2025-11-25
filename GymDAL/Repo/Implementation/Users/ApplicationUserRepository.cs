using GymDAL.Repo.Abstract.Users;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace GymDAL.Repo.Implementation
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        

        public ApplicationUserRepository(
            GymDbContext context,
            UserManager<ApplicationUser> userManager
            ,IMapper mapper) : base(context,mapper)
        {
            _userManager = userManager;
            
        }




        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password, string Createdby)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    user.CreatedBy = Createdby;
                    user.CreatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return user;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<ApplicationUser> GetByAsync(Expression<Func<ApplicationUser, bool>>? Filter)
        {
            try
            {
                IQueryable<ApplicationUser> query = _context.Users;
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                var user = query.FirstOrDefault();
                return Task.FromResult(user);
            }
            catch (Exception)
            {
                return Task.FromResult<ApplicationUser>(null);
            }
        }

        public Task<IEnumerable<ApplicationUser>> GetAsync(Expression<Func<ApplicationUser, bool>>? Filter)
        {
            try
            {
                IQueryable<ApplicationUser> query = _context.Users;
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                var users = query.AsEnumerable();
                return Task.FromResult(users);
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<ApplicationUser>>(null);
            }
        }
        public Task<ApplicationUser> GetUserWithNotificationsAsync(string userId)
        {
            try
            {
                var user = _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new ApplicationUser
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        // Copy other properties as needed
                        Notifications = u.Notifications
                    })
                    .FirstOrDefault();
                return Task.FromResult(user);
            }
            catch (Exception)
            {
                return Task.FromResult<ApplicationUser>(null);
            }
        }
        public Task<ApplicationUser> GetUserWithReportsAsync(string userId)
        {
            try
            {
                var user = _context.Users
              .Where(u => u.Id == userId)
              .Select(u => new ApplicationUser
              {
                  Id = u.Id,
                  UserName = u.UserName,
                  // Copy other properties as needed
                  GeneratedReports = u.GeneratedReports
              })
              .FirstOrDefault();
                return Task.FromResult(user);
            }
            catch (Exception)
            {
                return Task.FromResult<ApplicationUser>(null);

            }
        }


        public Task<bool> ToggleStatusUserAsync(string userId, string DeletedBy)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user == null) return Task.FromResult(false);
                if (user.ToggleStatus(DeletedBy))
                {
                    _context.Users.Update(user);
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



        public Task<ApplicationUser> UpdateUserAsync(ApplicationUser user, string UpdatedBy)
        {
            try
            {
                var existingUser = _context.Users.Find(user.Id);
                if (existingUser == null) return null;
                existingUser.UpdatedBy = UpdatedBy;
                existingUser.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(existingUser);
                _context.SaveChanges();
                return Task.FromResult(existingUser);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}