using GymDAL.Repo.Abstract.Users;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace GymDAL.Repo.Implementation.Users
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        

        public ApplicationUserRepository(
            GymDbContext context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper) : base(context, mapper)
        {
            _userManager = userManager;
            
        }

        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var createdUser = AddAsync(user);
                    if(createdUser.IsCompleted == true)
                    {
                        return user;

                    }
                  
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}